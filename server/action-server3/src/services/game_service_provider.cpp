#include "game_service_provider.h"

#include <memory>
#include <fmt/core.h>
#include <fmt/ranges.h>
#include <random>

#include "core/configured_eigen.h"
#include "utility/vector_utility.h"

#include "session/session.h"
#include "services/network_service_provider.h"

#include "proto/message.pb.h"
#include "proto/auth_login.pb.h"
#include "proto/chat_send_message.pb.h"
#include "proto/diagnosis_sever_sessions.pb.h"
#include "proto/diagnosis_ping_pong.pb.h"
#include "proto/diagnosis_gizmo.pb.h"
#include "proto/unit_spawn_ready.pb.h"
#include "proto/unit_spawn.pb.h"
#include "proto/unit_despawn.pb.h"
#include "proto/unit_move.pb.h"
#include "proto/unit_stop.pb.h"
#include "proto/unit_knockback.pb.h"
#include "proto/battle_skill_cast.pb.h"

#include "rpc.h"
#include "units/unit.h"
#include "units/unit_registory.h"

#include "area/area.h"

#include "user/user.h"
#include "user/user_registory.h"

#include "generated/cpp/auth_login.h"
#include "generated/cpp/chat_send_message.h"
#include "generated/cpp/diagnosis_sever_sessions.h"
#include "generated/cpp/diagnosis_ping_pong.h"
#include "generated/cpp/diagnosis_gizmo.h"
#include "generated/cpp/unit_spawn_ready.h"
#include "generated/cpp/unit_spawn.h"
#include "generated/cpp/unit_despawn.h"
#include "generated/cpp/unit_move.h"
#include "generated/cpp/unit_stop.h"
#include "generated/cpp/unit_knockback.h"
#include "generated/cpp/battle_skill_cast.h"

GameServiceProvider::GameServiceProvider(std::shared_ptr<Service> service)
	: _service(service)
	, _userRegistory(std::make_shared<potato::UserRegistory>())
	, _unitRegistory(std::make_shared<potato::UnitRegistory>())
{}

bool GameServiceProvider::isRunning()
{
	return true;
}

void GameServiceProvider::initialize()
{
	_service->getQueue().appendListener(ServiceProviderType::Game, [](const std::string& s, bool b) {
		fmt::print("received queue {}:{}\n", s, b);
		});

	_nerworkServiceProvider.lock()->setAcceptedDelegate([this](auto _) { onAccepted(_); });
	_nerworkServiceProvider.lock()->setSessionStartedDelegate([this](auto _) { onSessionStarted(_); });
	_nerworkServiceProvider.lock()->setDisconnectedDelegate([this](auto _) { onDisconnected(_); });

	_userRegistory->setOnUnregisterUser([this](auto user) {
		auto unit = _unitRegistory->findUnitByUnitId(user->getUnitId());
		sendDespawn(potato::net::SessionId(0), unit);

		auto areaId = unit->getAreaId();
		auto areaIt = std::find_if(_areas.begin(), _areas.end(), [areaId](auto& area) { return area->getAreaId() == areaId; });
		if (areaIt != _areas.end())
		{
			(*areaIt)->leave(unit);
		}

		_unitRegistory->unregisterUnit(unit);

		auto& user_index = _idMapper.get<user_id>();
		auto binderIt = user_index.find(user->getUserId());
		_idMapper.erase(binderIt);
		});

	{
		auto addToArea = [this](AreaId areaId, std::shared_ptr<Unit> newUnit) {
			std::shared_ptr<potato::Area> area;
			auto areaIt = std::find_if(_areas.begin(), _areas.end(), [areaId](auto& area) { return area->getAreaId() == areaId; });
			if (areaIt == _areas.end())
			{
				area = _areas.emplace_back(std::make_shared<potato::Area>(areaId));
			}
			else
			{
				area = *areaIt;
			}
			area->enter(newUnit);
		};

		//for (float p = -20; p < 20; p += 0.005f)
		//for (float p = -20; p < 20; p += 0.05f)
		for (float p = -20; p < 20; p += 3.0f)
		//float p = 0;
		{
			auto newUnit = _unitRegistory->createUnit(potato::net::session::getSystemSessionId());
			newUnit->setPosition({ p, 0, 0 });
			newUnit->setDisplayName(fmt::format("NONAME{}", newUnit->getUnitId()));
			newUnit->setUnitAction([this](auto unit, auto now) {
				auto getMoveDirection = [](const Eigen::Vector3f& direction) {
					if (direction.y() == 0)
					{
						return direction.x() > 0 ? potato::UnitDirection::UNIT_DIRECTION_RIGHT : potato::UnitDirection::UNIT_DIRECTION_LEFT;
					}
					else
					{
						return direction.y() < 0 ? potato::UnitDirection::UNIT_DIRECTION_DOWN : potato::UnitDirection::UNIT_DIRECTION_UP;
					}
				};

				if (!unit->isMoving() && ((now / 1000) % 5) == 0)
				{
					auto moveCommand = std::make_shared<MoveCommand>();
					moveCommand->startTime = now;
					const auto from = unit->getTrackbackPosition(now);
					Eigen::Vector3f randomDirection;

					if (((now / 1000) % 15) != 0)
					{
						std::random_device rd;
						std::default_random_engine eng(rd());
						std::uniform_real_distribution<float> distr(-1, 1);
						randomDirection << distr(eng), distr(eng), 0;
						randomDirection.normalize();
					}
					else
					{
						randomDirection = -from.normalized();
					}

					const auto to = from + randomDirection * 500;
					moveCommand->from = from;
					moveCommand->to = to;
					moveCommand->speed = 0.0025f;
					moveCommand->direction = getMoveDirection(randomDirection);
					moveCommand->moveId = 0;
					unit->inputCommand(moveCommand);
					sendMove(potato::net::SessionId(0), unit, moveCommand);

					auto stopCommand = std::make_shared<StopCommand>();
					stopCommand->stopTime = now + 2000;
					stopCommand->direction = moveCommand->direction;
					stopCommand->moveId = 0;
					unit->inputCommand(stopCommand);
					sendStop(potato::net::SessionId(0), unit, stopCommand);

					//const auto expectStop = moveCommand->getPosition(stopCommand->stopTime);
					//fmt::print("from: {}, {}, {}  to:{}, {}, {}  expectStop:{}, {}, {}\n",
					//	from.x(), from.y(), from.z(),
					//	to.x(), to.y(), to.z(),
					//	expectStop.x(), expectStop.y(), expectStop.z()
					//);
				}
				});
			addToArea(0, newUnit);
		}
	}
}

void GameServiceProvider::onAccepted(std::shared_ptr<potato::net::session> session)
{
	auto authLogin = std::make_shared<torikime::auth::login::Rpc>(session);
	auto weakSession = std::weak_ptr(session);
	authLogin->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			UserAuthenticator authenticator;
			auto r = authenticator.DoAuth(requestParcel.request().user_id(), requestParcel.request().password());
			if (r.has_value())
			{
				const std::string user_id_name = requestParcel.request().user_id();
				queue.enqueue(0, [this, session, responser, user_id_name, r]() {
					torikime::auth::login::Response response;

					std::shared_ptr<potato::User> user;
					auto& user_index = _idMapper.get<user_id>();
					auto binderIt = user_index.find(r.value());
					if (binderIt != user_index.end())
					{
						// rebind session
						user_index.replace(binderIt, { r.value(), session->getSessionId(), binderIt->unitId });
						user = _userRegistory->find(r.value());
					}
					else
					{
						// new session
						_idMapper.insert({ r.value(), session->getSessionId(), UnitId(0) });
						user = _userRegistory->registerUser(r.value());
					}
					user->setSession(session);
					user->setUnitId(binderIt->unitId);
					user->setDisplayName(user_id_name);

					fmt::print("session id[{}] user_id: {}({}) logged in\n", session->getSessionId(), r.value(), user_id_name);
					response.set_ok(true);
					responser->send(true, std::move(response));
				});
			}
			else
			{
				torikime::auth::login::Response response;
				response.set_ok(false);
				responser->send(false, std::move(response));
			}
			});
	_nerworkServiceProvider.lock()->registerRpc(authLogin);

	auto chat = std::make_shared<torikime::chat::send_message::Rpc>(session);
	std::weak_ptr<torikime::chat::send_message::Rpc> weak_chat = chat;
	chat->subscribeRequest([this, weak_chat, weakSession](const torikime::chat::send_message::RequestParcel& requestParcel, std::shared_ptr<torikime::chat::send_message::Responser>& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			//std::cout << "receive RequestParcel\n";
			const auto message = requestParcel.request().message();
			torikime::chat::send_message::Response response;
			const int64_t messageId = 0;
			response.set_message_id(messageId);
			responser->send(true, std::move(response));

			torikime::chat::send_message::Notification notification;
			notification.set_message(message);
			notification.set_message_id(messageId);
			notification.set_from(fmt::to_string(session->getSessionId()));
			_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), weak_chat.lock()->serializeNotification(notification)); });
	_nerworkServiceProvider.lock()->registerRpc(chat);

	auto diagnosis = std::make_shared<torikime::diagnosis::sever_sessions::Rpc>(session);
	diagnosis->subscribeRequest([this](const torikime::diagnosis::sever_sessions::RequestParcel&, std::shared_ptr<torikime::diagnosis::sever_sessions::Responser>& responser)
		{
			torikime::diagnosis::sever_sessions::Response response;
			response.set_session_count(_nerworkServiceProvider.lock()->getConnectionCount());
			responser->send(true, std::move(response)); });
	_nerworkServiceProvider.lock()->registerRpc(diagnosis);

	auto pingPong = std::make_shared<torikime::diagnosis::ping_pong::Rpc>(session);
	pingPong->subscribeRequest([this, pingPong, weakSession](const torikime::diagnosis::ping_pong::RequestParcel& requestParcel, std::shared_ptr<torikime::diagnosis::ping_pong::Responser>& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			auto& units = _unitRegistory->getUnits();
			auto unit = std::find_if(units.begin(), units.end(), [this, &units, &pingPong, requestParcel](auto& u) {
				return pingPong->getSession()->getSessionId() == u->getSessionId();
				});
			if (unit != units.end())
			{
				(*unit)->setLastLatency(requestParcel.request().last_latency());
				//fmt::print("unit[{}] last latency: {}\n", (*unit)->getUnitId(), (*unit)->getLastLatency());
			}

			torikime::diagnosis::ping_pong::Response response;
			response.set_receive_time(std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count());
			response.set_send_time(std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count());
			responser->send(true, std::move(response));
		});
	_nerworkServiceProvider.lock()->registerRpc(pingPong);

	auto unitSpawnReady = std::make_shared<torikime::unit::spawn_ready::Rpc>(session);
	unitSpawnReady->subscribeRequest([this, weakSession](const auto& request, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			bool rebind = false;
			std::shared_ptr<Unit> newUnit;
			auto& session_index = _idMapper.get<potato::net::session_id>();
			auto binderIt = session_index.find(session->getSessionId());
			if (binderIt != session_index.end() && binderIt->unitId != UnitId(0))
			{
				newUnit = _unitRegistory->findUnitByUnitId(binderIt->unitId);
				newUnit->setSessionId(session->getSessionId());
				rebind = true;
			}
			else
			{
				newUnit = _unitRegistory->createUnit(session->getSessionId());
			}

			auto user = _userRegistory->find(binderIt->userId);
			newUnit->setDisplayName(user->getDisplayName());

			// update unit id
			if (binderIt != session_index.end())
			{
				session_index.replace(binderIt, { binderIt->userId, binderIt->sessionId, newUnit->getUnitId() });
				auto user = _userRegistory->find(binderIt->userId);
				user->setUnitId(newUnit->getUnitId());
			}

			const auto areaId = static_cast<AreaId>(request.request().area_id());
			// response
			{
				std::shared_ptr<potato::Area> area;
				auto areaIt = std::find_if(_areas.begin(), _areas.end(), [areaId](auto& area) { return area->getAreaId() == areaId; });
				if (areaIt == _areas.end())
				{
					area = _areas.emplace_back(std::make_shared<potato::Area>(areaId));
				}
				else
				{
					area = *areaIt;
				}
				area->enter(newUnit);

				torikime::unit::spawn_ready::Response response;
				response.set_session_id(session->getSessionId().value_of());
				response.set_unit_id(newUnit->getUnitId().value_of());
				response.set_allocated_position(newVector3(newUnit->getPosition()));
				response.set_direction(newUnit->getDirection());
				auto individuality = new potato::Individuality();
				individuality->set_type(potato::UNIT_TYPE_PLAYER);
				response.set_allocated_individuality(individuality);
				response.set_cause(potato::UNIT_SPAWN_CAUSE_LOGGEDIN);
				auto avatar = new potato::Avatar();
				avatar->set_name(user->getDisplayName());
				response.set_allocated_avatar(avatar);
				responser->send(true, std::move(response));
			}

			if (!rebind)
			{
				sendSpawnUnit(session->getSessionId(), newUnit);
			}
		});
	_nerworkServiceProvider.lock()->registerRpc(unitSpawnReady);

	auto unitMove = std::make_shared<torikime::unit::move::Rpc>(session);
	unitMove->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			{
				torikime::unit::move::Response response;
				response.set_ok(true);
				responser->send(true, std::move(response));
			}

			const auto& units = _unitRegistory->getUnits();
			auto unit = std::find_if(units.begin(), units.end(), [requestParcel](auto& u) {
				return requestParcel.request().unit_id() == u->getUnitId().value_of();
				});
			if (unit != units.end())
			{
				auto moveCommand = std::make_shared<MoveCommand>();
				moveCommand->startTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
				moveCommand->from = toVector3f(requestParcel.request().from());
				moveCommand->to = toVector3f(requestParcel.request().to());
				moveCommand->speed = requestParcel.request().speed();
				moveCommand->direction = requestParcel.request().direction();
				moveCommand->moveId = requestParcel.request().move_id();
				(*unit)->inputCommand(moveCommand);
			}

			{
				torikime::unit::move::Notification notification;
				notification.set_unit_id(requestParcel.request().unit_id());
				notification.set_time(requestParcel.request().time());
				auto from = requestParcel.request().from();
				auto to = requestParcel.request().to();
				notification.set_allocated_from(&from);
				notification.set_allocated_to(&to);
				notification.set_speed(requestParcel.request().speed());
				notification.set_direction(requestParcel.request().direction());
				notification.set_move_id(requestParcel.request().move_id());
				_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), torikime::unit::move::Rpc::serializeNotification(notification));
				notification.release_to();
				notification.release_from();
			}
		});
	_nerworkServiceProvider.lock()->registerRpc(unitMove);

	auto unitStop = std::make_shared<torikime::unit::stop::Rpc>(session);
	unitStop->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			{
				torikime::unit::stop::Response response;
				response.set_ok(true);
				responser->send(true, std::move(response));
			}

			const auto& units = _unitRegistory->getUnits();
			auto unit = std::find_if(units.begin(), units.end(), [requestParcel](auto& u) {
				return requestParcel.request().unit_id() == u->getUnitId().value_of();
				});
			if (unit != units.end())
			{
				auto stopCommand = std::make_shared<StopCommand>();
				stopCommand->stopTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
				stopCommand->direction = requestParcel.request().direction();
				stopCommand->moveId = requestParcel.request().move_id();
				(*unit)->inputCommand(stopCommand);
			}

			{
				torikime::unit::stop::Notification notification;
				notification.set_unit_id(requestParcel.request().unit_id());
				notification.set_time(requestParcel.request().time());
				notification.set_stop_time(requestParcel.request().stop_time());
				notification.set_direction(requestParcel.request().direction());
				notification.set_move_id(requestParcel.request().move_id());
				_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), torikime::unit::stop::Rpc::serializeNotification(notification));
			}
		});
	_nerworkServiceProvider.lock()->registerRpc(unitStop);

	{
		auto battleSkillCast = std::make_shared<torikime::battle::skill_cast::Rpc>(session);
		battleSkillCast->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
			{
				using namespace torikime::battle::skill_cast;

				auto session = weakSession.lock();
				assert(session);

				uint64_t attackId = ++_attackId;
				uint32_t skillId = requestParcel.request().skill_id();
				int64_t triggerTime = requestParcel.request().trigger_time();
				Response response;
				response.set_ok(true);
				response.set_attack_id(attackId);
				responser->send(true, std::move(response));

				queue.enqueue(0, [this, session, attackId, skillId, triggerTime]() {
					const auto& units = _unitRegistory->getUnits();

					std::vector<std::shared_ptr<potato::net::protocol::Payload>> knockbackPayloads;

					{
						auto casterUnit = _unitRegistory->findUnitBySessionId(session->getSessionId());
						fmt::print("attack reveived caster:{} trigger_time:{} skill_id:{} attack_id:{}\n", casterUnit->getUnitId(), triggerTime, skillId, attackId);

						Notification notification;
						notification.set_caster_unit_id(casterUnit->getUnitId().value_of());
						notification.set_trigger_time(triggerTime);
						notification.set_skill_id(skillId);
						notification.set_attack_id(attackId);
						{
							auto casterUnitPositionAtTheTime = casterUnit->getTrackbackPosition(triggerTime);
							for (auto unit : units)
							{
								if (unit->getUnitId() == casterUnit->getUnitId()
									|| unit->getAreaId() != casterUnit->getAreaId())
								{
									continue;
								}

								auto receiverUnitPositionAtTheTime = unit->getTrackbackPosition(triggerTime);
								auto range = 1.0f;
								if ((receiverUnitPositionAtTheTime - casterUnitPositionAtTheTime).squaredNorm() > range * range)
								{
									fmt::print("too far {} > {}\n", (receiverUnitPositionAtTheTime - casterUnitPositionAtTheTime).norm(), range);
									continue;
								}

								fmt::print("cast skill hit {} to {}\n", casterUnit->getUnitId(), unit->getUnitId());
								auto result = notification.add_results();
								result->set_receiver_unit_id(unit->getUnitId().value_of());
								result->set_damage(100);
								result->set_heal(0);
								result->set_miss(false);
								result->set_dodged(false);

								auto knockbackDirection = (receiverUnitPositionAtTheTime - casterUnitPositionAtTheTime).normalized();
								auto knockbackCommand = std::make_shared<KnockbackCommand>();
								int64_t knockbackDuration = 200;
								knockbackCommand->startTime = triggerTime;
								knockbackCommand->endTime = triggerTime + knockbackDuration;
								knockbackCommand->moveId = 0;
								knockbackCommand->from = receiverUnitPositionAtTheTime;
								knockbackCommand->to = receiverUnitPositionAtTheTime + knockbackDirection * 1;
								knockbackCommand->lastMoveCommand = unit->getLastMoveCommand();
								knockbackCommand->direction = unit->getDirection();
								knockbackCommand->speed = (knockbackCommand->to - knockbackCommand->from).norm() / static_cast<float>(knockbackDuration);
								unit->inputCommand(knockbackCommand);

								{
									torikime::unit::knockback::Notification notification;
									notification.set_unit_id(unit->getUnitId().value_of());
									notification.set_start_time(knockbackCommand->startTime);
									notification.set_end_time(knockbackCommand->endTime);
									notification.set_allocated_from(newVector3(knockbackCommand->from));
									notification.set_allocated_to(newVector3(knockbackCommand->to));
									notification.set_speed(knockbackCommand->speed);
									notification.set_direction(knockbackCommand->direction);
									notification.set_move_id(knockbackCommand->moveId);
									knockbackPayloads.emplace_back(torikime::unit::knockback::Rpc::serializeNotification(notification));
								}

								auto stopCommand = std::make_shared<StopCommand>();
								stopCommand->stopTime = knockbackCommand->endTime;
								stopCommand->direction = knockbackCommand->direction;
								stopCommand->moveId = 0;
								unit->inputCommand(stopCommand);

								{
									torikime::unit::stop::Notification notification;
									notification.set_unit_id(unit->getUnitId().value_of());
									notification.set_time(knockbackCommand->endTime);
									notification.set_stop_time(knockbackCommand->endTime);
									notification.set_direction(knockbackCommand->direction);
									notification.set_move_id(0);
									knockbackPayloads.emplace_back(torikime::unit::stop::Rpc::serializeNotification(notification));
								}
							}
						}
						// TODO: change to sendAreacast
						_nerworkServiceProvider.lock()->sendBroadcast(potato::net::SessionId(0), Rpc::serializeNotification(notification));
						for (auto& payload : knockbackPayloads)
						{
							_nerworkServiceProvider.lock()->sendBroadcast(potato::net::SessionId(0), payload);
						}
					}

					});
			});
		_nerworkServiceProvider.lock()->registerRpc(battleSkillCast);
	}
}

void GameServiceProvider::onSessionStarted(std::shared_ptr<potato::net::session>)
{
}

void GameServiceProvider::onDisconnected(std::shared_ptr<potato::net::session> session)
{
	// update user id
	auto& session_index = _idMapper.get<potato::net::session_id>();
	auto binderIt = session_index.find(session->getSessionId());
	if (binderIt != session_index.end())
	{
		session_index.replace(binderIt, { binderIt->userId, potato::net::SessionId(0), binderIt->unitId });
		auto user = _userRegistory->find(binderIt->userId);
		user->clearSession();
	}
}

// TODO: move to message service
void GameServiceProvider::sendSystemMessage(const std::string& message)
{
	torikime::chat::send_message::Notification notification;
	notification.set_message(message);
	notification.set_message_id(++messageId);
	notification.set_from("system");
	// session id 0 is system
	_nerworkServiceProvider.lock()->sendBroadcast(potato::net::SessionId(0), torikime::chat::send_message::Rpc::serializeNotification(notification));
}

void GameServiceProvider::sendSpawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> spawnUnit)
{
	{
		// broadcast spawn to neighbors
		torikime::unit::spawn::Notification notification;
		notification.set_session_id(sessionId.value_of());
		notification.set_unit_id(spawnUnit->getUnitId().value_of());
		notification.set_area_id(spawnUnit->getAreaId());
		notification.set_allocated_position(newVector3({ 0, 0, 0 }));
		notification.set_direction(potato::UNIT_DIRECTION_DOWN);
		auto individuality = new potato::Individuality();
		individuality->set_type(potato::UNIT_TYPE_PLAYER);
		notification.set_allocated_individuality(individuality);
		notification.set_cause(potato::UNIT_SPAWN_CAUSE_LOGGEDIN);
		auto avatar = new potato::Avatar();
		avatar->set_name(spawnUnit->getDisplayName());
		notification.set_allocated_avatar(avatar);
		_nerworkServiceProvider.lock()->sendBroadcast(sessionId, torikime::unit::spawn::Rpc::serializeNotification(notification));

		for (const auto unit : _unitRegistory->getUnits())
		{
			if (unit->getUnitId() == spawnUnit->getUnitId())
			{
				continue;
			}
			// spawn
			{
				torikime::unit::spawn::Notification notification;
				notification.set_session_id(unit->getSessionId().value_of());
				notification.set_unit_id(unit->getUnitId().value_of());
				notification.set_area_id(unit->getAreaId());
				notification.set_allocated_position(newVector3(unit->getPosition()));
				notification.set_direction(potato::UNIT_DIRECTION_DOWN);
				auto individuality = new potato::Individuality();
				individuality->set_type(potato::UNIT_TYPE_PLAYER);
				notification.set_allocated_individuality(individuality);
				notification.set_cause(potato::UNIT_SPAWN_CAUSE_ASIS);
				auto avatar = new potato::Avatar();
				avatar->set_name(unit->getDisplayName());
				notification.set_allocated_avatar(avatar);
				auto payload = torikime::unit::spawn::Rpc::serializeNotification(notification);
				_nerworkServiceProvider.lock()->sendTo(sessionId, payload);
			}

			// current move state
			{
				auto moveCommand = unit->getLastMoveCommand();
				if (moveCommand != nullptr)
				{
					torikime::unit::move::Notification notification;
					notification.set_unit_id(unit->getUnitId().value_of());
					notification.set_time(moveCommand->startTime);
					notification.set_allocated_from(newVector3(moveCommand->from));
					notification.set_allocated_to(newVector3(moveCommand->to));
					notification.set_speed(moveCommand->speed);
					notification.set_direction(moveCommand->direction);
					notification.set_move_id(moveCommand->moveId);
					_nerworkServiceProvider.lock()->sendTo(sessionId, torikime::unit::move::Rpc::serializeNotification(notification));
				}

				auto stopCommand = std::dynamic_pointer_cast<StopCommand>(unit->getLastCommand());
				if (stopCommand != nullptr)
				{
					torikime::unit::stop::Notification notification;
					notification.set_unit_id(unit->getUnitId().value_of());
					auto lastMoveCommand = stopCommand->lastMoveCommand.lock();
					auto lastMoveTime = lastMoveCommand != nullptr ? lastMoveCommand->startTime : 0;
					notification.set_time(lastMoveTime);
					notification.set_stop_time(stopCommand->stopTime);
					notification.set_direction(stopCommand->direction);
					notification.set_move_id(stopCommand->moveId);

					auto payload = torikime::unit::stop::Rpc::serializeNotification(notification);
					_nerworkServiceProvider.lock()->sendTo(sessionId, payload);
				}
			}
		}
	}
}

void GameServiceProvider::sendDespawn(potato::net::SessionId sessionId, std::shared_ptr<Unit> despawnUnit)
{
	if (despawnUnit == nullptr)
	{
		fmt::print("session[{}] call sendDespawn but unit is null\n", sessionId);
		return;
	}
	auto unitDespawn = std::make_shared<torikime::unit::despawn::Rpc>(std::shared_ptr<potato::net::session>());
	torikime::unit::despawn::Notification notification;
	notification.set_session_id(sessionId.value_of());
	notification.set_unit_id(despawnUnit->getUnitId().value_of());
	_nerworkServiceProvider.lock()->sendBroadcast(sessionId, torikime::unit::despawn::Rpc::serializeNotification(notification));
}

void GameServiceProvider::sendMove(potato::net::SessionId sessionId, std::shared_ptr<Unit> unit, std::shared_ptr<MoveCommand> moveCommand)
{
	if (moveCommand != nullptr)
	{
		torikime::unit::move::Notification notification;
		notification.set_unit_id(unit->getUnitId().value_of());
		notification.set_time(moveCommand->startTime);
		notification.set_allocated_from(newVector3(moveCommand->from));
		notification.set_allocated_to(newVector3(moveCommand->to));
		notification.set_speed(moveCommand->speed);
		notification.set_direction(moveCommand->direction);
		notification.set_move_id(moveCommand->moveId);
		_nerworkServiceProvider.lock()->sendBroadcast(sessionId, torikime::unit::move::Rpc::serializeNotification(notification));
	}
}

void GameServiceProvider::sendStop(potato::net::SessionId sessionId, std::shared_ptr<Unit> unit, std::shared_ptr<StopCommand> stopCommand)
{
	if (stopCommand != nullptr)
	{
		torikime::unit::stop::Notification notification;
		notification.set_unit_id(unit->getUnitId().value_of());
		auto lastMoveCommand = stopCommand->lastMoveCommand.lock();
		auto lastMoveTime = lastMoveCommand != nullptr ? lastMoveCommand->startTime : 0;
		notification.set_time(lastMoveTime);
		notification.set_stop_time(stopCommand->stopTime);
		notification.set_direction(stopCommand->direction);
		notification.set_move_id(stopCommand->moveId);

		auto payload = torikime::unit::stop::Rpc::serializeNotification(notification);
		_nerworkServiceProvider.lock()->sendBroadcast(sessionId, payload);
	}
}

void GameServiceProvider::main()
{
	auto prev = std::chrono::high_resolution_clock::now();
	auto nextSecond = std::chrono::high_resolution_clock::now() + std::chrono::seconds(1);
	queue.appendListener(0, [](std::function<void()> f) { f(); });
	int32_t fps = 0;
	std::vector<int32_t> frameProcessingTime;
	while (_running)
	{
		const auto nowUpdate = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
		for (auto& unit : _unitRegistory->getUnits())
		{
			unit->update(nowUpdate);

			//{
			//	torikime::diagnosis::gizmo::Notification notification;
			//	notification.set_name(fmt::format("unit[{}]", unit->getUnitId()));
			//	auto from = new potato::Vector3();
			//	from->set_x(0);
			//	from->set_y(0);
			//	from->set_z(0);
			//	auto to = new potato::Vector3();
			//	to->set_x(unit->getPosition().x());
			//	to->set_y(unit->getPosition().y());
			//	to->set_z(unit->getPosition().z());
			//	notification.set_allocated_begin(from);
			//	notification.set_allocated_end(to);
			//	notification.set_color(0xffffffff);
			//	_nerworkServiceProvider.lock()->sendBroadcast(0, torikime::diagnosis::gizmo::Rpc::serializeNotification(notification));
			//}
		}

		queue.process();
		_service->getQueue().process();

		sendSystemMessage("hey");

		{
			_userRegistory->update(nowUpdate);
		}

		{
			fps++;

			const auto now = std::chrono::high_resolution_clock::now();
			if (now >= nextSecond)
			{
				fmt::print("fps: {}, microsec/frame: [{:5}]\n", fps, fmt::join(frameProcessingTime, ","));
				frameProcessingTime.clear();
				fps = 0;
				nextSecond = std::chrono::high_resolution_clock::now() + std::chrono::seconds(1);
			}
			const auto spareTime = std::chrono::high_resolution_clock::now() - prev;
			std::this_thread::sleep_for(std::max(std::chrono::nanoseconds(0), std::chrono::milliseconds(100) - spareTime));
			prev = std::chrono::high_resolution_clock::now();

			// fmt::print("spareTime: {}microseconds\n", std::chrono::duration_cast<std::chrono::microseconds>(spareTime).count());
			frameProcessingTime.emplace_back(std::chrono::duration_cast<std::chrono::microseconds>(spareTime).count());
		}
	}
}

void GameServiceProvider::start()
{
	_thread = std::thread([this]() {
		_nerworkServiceProvider = _service->findServiceProvider<NetworkServiceProvider>();
		initialize();
		fmt::print("start game service loop\n");
		main();
		fmt::print("end game service loop\n");
		});
}

void GameServiceProvider::stop()
{
	_running = false;
	_thread.join();
}
