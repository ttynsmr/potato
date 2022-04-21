#include "game_service_provider.h"

#include <fmt/core.h>
#include <fmt/ranges.h>

#include "core/configured_eigen.h"
#include "utility/vector_utility.h"

#include "session/session.h"
#include "services/network_service_provider.h"

#include "message.pb.h"
#include "area_transport.pb.h"
#include "area_constituted_data.pb.h"
#include "auth_login.pb.h"
#include "chat_send_message.pb.h"
#include "diagnosis_sever_sessions.pb.h"
#include "diagnosis_ping_pong.pb.h"
#include "diagnosis_gizmo.pb.h"
#include "unit_spawn_ready.pb.h"
#include "unit_spawn.pb.h"
#include "unit_despawn.pb.h"
#include "unit_move.pb.h"
#include "unit_stop.pb.h"
#include "unit_knockback.pb.h"
#include "battle_skill_cast.pb.h"
#include "battle_sync_parameters.pb.h"

#include "rpc/rpc.h"

#include "units/unit.h"
#include "units/unit_registry.h"
#include "units/components/npc_component.h"
#include "units/components/status_component.h"

#include "area/area.h"
#include "area/area_registry.h"

#include "node/node.h"

#include "user/user.h"
#include "user/user_registry.h"

#include "area_transport.h"
#include "area_constituted_data.h"
#include "auth_login.h"
#include "chat_send_message.h"
#include "diagnosis_sever_sessions.h"
#include "diagnosis_ping_pong.h"
#include "diagnosis_gizmo.h"
#include "unit_spawn_ready.h"
#include "unit_spawn.h"
#include "unit_despawn.h"
#include "unit_move.h"
#include "unit_stop.h"
#include "unit_knockback.h"
#include "battle_skill_cast.h"
#include "battle_sync_parameters.h"

#include "rpc_builder.h"

GameServiceProvider::GameServiceProvider()
	: _userRegistry(std::make_shared<potato::UserRegistry>())
	, _unitRegistry(std::make_shared<potato::UnitRegistry>())
	, _areaRegistry(std::make_shared<potato::AreaRegistry>())
	, _randomEngine(_randomDevice())
{
}

bool GameServiceProvider::isRunning()
{
	return true;
}

void GameServiceProvider::initialize()
{
	_nerworkServiceProvider = ServiceRegistry::instance().findServiceProvider<NetworkServiceProvider>();

	ServiceRegistry::instance().getQueue().appendListener(ServiceProviderType::Game, [](auto action) {
		fmt::print("received queue\n");
		action();
		});

	auto nerworkServiceProvider = _nerworkServiceProvider.lock();
	nerworkServiceProvider->setAcceptedDelegate([this](auto _) { onAccepted(_); });
	nerworkServiceProvider->setSessionStartedDelegate([this](auto _) { onSessionStarted(_); });
	nerworkServiceProvider->setDisconnectedDelegate([this](auto _) { onDisconnected(_); });

	_rpcBuilder = std::make_shared<RpcBuilder>();

	_userRegistry->setOnUnregisterUser([this](auto user) { onUnregisterUser(user); });

	generateNPCs();
}

void GameServiceProvider::generateNPCs()
{
	auto addToArea = [this](potato::AreaId areaId, std::shared_ptr<Unit> newUnit) {
		std::shared_ptr<potato::Area> area = _areaRegistry->getArea(areaId);
		if (!area)
		{
			area = _areaRegistry->addArea(areaId);
			area->requestLoad();
		}
		area->enter(newUnit);
	};

	//for (float p = -20; p < 20; p += 0.005f)
	//for (float p = -20; p < 20; p += 0.05f)
	for (float p = -20; p < 20; p += 3.0f)
		//float p = 0;
	{
		auto newUnit = _unitRegistry->createUnit(potato::net::Session::getSystemSessionId());
		newUnit->setPosition({ p, 0, 0 });
		newUnit->setDisplayName(fmt::format("NONAME{}", newUnit->getUnitId()));
		newUnit->addComponent<NpcComponent>(shared_from_this());
		newUnit->addComponent<StatusComponent>(shared_from_this(), _nerworkServiceProvider.lock());
		addToArea(potato::AreaId(0), newUnit);
	}
}

void GameServiceProvider::onUnregisterUser(std::shared_ptr<potato::User> user)
{
	auto unit = _unitRegistry->findUnitByUnitId(user->getUnitId());
	if (unit)
	{
		sendAreacastDespawnUnit(potato::net::SessionId(0), unit);

		auto areaId = unit->getAreaId();
		auto area = _areaRegistry->getArea(areaId);
		area->leave(unit);
		
		_unitRegistry->unregisterUnit(unit);

		const auto now = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
		unit->onDespawn(now);
	}

	auto& user_index = _idMapper.get<user_id>();
	auto binderIt = user_index.find(user->getUserId());
	_idMapper.erase(binderIt);
}

void GameServiceProvider::onAccepted(std::shared_ptr<potato::net::Session> session)
{
	_rpcBuilder->build(_nerworkServiceProvider.lock(), session);

	auto weakSession = std::weak_ptr(session);
	_rpcBuilder->auth.login->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
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
						if (potato::net::SessionId(0) != binderIt->sessionId)
						{
							// disconnect old session
							_nerworkServiceProvider.lock()->disconnectSession(binderIt->sessionId);
						}

						// rebind session
						user_index.replace(binderIt, { r.value(), session->getSessionId(), binderIt->unitId });
						user = _userRegistry->find(r.value());
					}
					else
					{
						// new session
						_idMapper.insert({ r.value(), session->getSessionId(), UnitId(0) });
						user = _userRegistry->registerUser(r.value());
					}
					user->setSession(session);
					user->setUnitId(binderIt->unitId);
					user->setDisplayName(user_id_name);

					fmt::print("session id[{}] user_id: {}({}) logged in\n", session->getSessionId(), r.value(), user_id_name);
					response.set_ok(true);
					responser->send(true, std::move(response));

					torikime::area::transport::Notification notification;
					notification.set_area_id(0);
					notification.set_unit_id(user->getUnitId().value_of());
					_nerworkServiceProvider.lock()->sendTo(user->getSessionId(), torikime::area::transport::Rpc::serializeNotification(notification));
				});
			}
			else
			{
				torikime::auth::login::Response response;
				response.set_ok(false);
				responser->send(false, std::move(response));
			}
			});

	std::weak_ptr<torikime::chat::send_message::Rpc> weak_chat = _rpcBuilder->chat.sendMessage;
	_rpcBuilder->chat.sendMessage->subscribeRequest([this, weak_chat, weakSession](const auto& requestParcel, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			//std::cout << "receive RequestParcel\n";
			const auto& message = requestParcel.request().message();
			torikime::chat::send_message::Response response;
			const int64_t messageId = 0;
			response.set_message_id(messageId);
			responser->send(true, std::move(response));

			torikime::chat::send_message::Notification notification;
			notification.set_message(message);
			notification.set_message_id(messageId);
			notification.set_from(fmt::to_string(session->getSessionId()));
			_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), weak_chat.lock()->serializeNotification(notification));
		});

	_rpcBuilder->diagnosis.severSessions->subscribeRequest([this](const auto&, auto& responser)
		{
			torikime::diagnosis::sever_sessions::Response response;
			response.set_session_count(_nerworkServiceProvider.lock()->getConnectionCount());
			responser->send(true, std::move(response));
		});

	auto& pingPong = _rpcBuilder->diagnosis.pingPong;
	pingPong->subscribeRequest([this, pingPong, weakSession](const auto& requestParcel, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			auto& units = _unitRegistry->getUnits();
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

	_rpcBuilder->area.transport->subscribeRequest([this](const auto& requestParcel, auto& responser)
		{
			responser->send(true, torikime::area::transport::Response());
			_onTransportRequest(requestParcel.request().transport_id());
		});

	_rpcBuilder->area.constitutedData->subscribeRequest([this](const auto& requestParcel, auto& responser)
		{
			auto area = _areaRegistry->getArea(potato::AreaId(requestParcel.request().area_id()));
			torikime::area::constituted_data::Response response;
			response.set_area_id(area->getAreaId().value_of());

			std::vector<std::shared_ptr<potato::TriggerableComponent>> triggerableComponents;
			std::function<void(std::shared_ptr<potato::Node>& node)> collectTriggerableComponent;
			collectTriggerableComponent = [&collectTriggerableComponent, &triggerableComponents](std::shared_ptr<potato::Node>& node) {
				auto triggerableComponent = node->getComponent<potato::TriggerableComponent>();
				if (triggerableComponent)
				{
					triggerableComponents.emplace_back(triggerableComponent);
				}
				node->process(collectTriggerableComponent);
			};
			area->getNodeRoot()->process(collectTriggerableComponent);
			for (auto& triggerableComponent : triggerableComponents)
			{
				auto triggers = response.add_triggers();
				triggers->set_area_id(area->getAreaId().value_of());
				triggers->set_allocated_position(newVector3(triggerableComponent->position));
				triggers->set_allocated_offset(newVector3(triggerableComponent->offset));
				triggers->set_allocated_size(newVector3(triggerableComponent->size));
			}
			responser->send(true, std::move(response));
		});

	_rpcBuilder->unit.spawnReady->subscribeRequest([this, weakSession](const auto& request, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			bool rebind = false;
			std::shared_ptr<Unit> newUnit;
			auto& session_index = _idMapper.get<potato::net::session_id>();
			auto binderIt = session_index.find(session->getSessionId());
			if (binderIt != session_index.end() && binderIt->unitId != UnitId(0))
			{
				newUnit = _unitRegistry->findUnitByUnitId(binderIt->unitId);
				newUnit->setSessionId(session->getSessionId());
				rebind = true;
			}
			else
			{
				newUnit = _unitRegistry->createUnit(session->getSessionId());
				newUnit->addComponent<StatusComponent>(shared_from_this(), _nerworkServiceProvider.lock());
			}

			auto user = _userRegistry->find(binderIt->userId);
			newUnit->setDisplayName(user->getDisplayName());

			// update unit id
			if (binderIt != session_index.end())
			{
				session_index.replace(binderIt, { binderIt->userId, binderIt->sessionId, newUnit->getUnitId() });
				auto user = _userRegistry->find(binderIt->userId);
				user->setUnitId(newUnit->getUnitId());
			}

			const auto areaId = static_cast<potato::AreaId>(request.request().area_id());
			// response
			{
				std::shared_ptr<potato::Area> area = _areaRegistry->getArea(areaId);
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
				sendAreacastSpawnUnit(session->getSessionId(), newUnit);
			}
			sendSpawnUnit(session->getSessionId(), newUnit);
			_onSpawnReadyRequest();
		});

	_rpcBuilder->unit.move->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			{
				torikime::unit::move::Response response;
				response.set_ok(true);
				responser->send(true, std::move(response));
			}

			const auto& units = _unitRegistry->getUnits();
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
				notification.set_area_id((*unit)->getAreaId().value_of());
				notification.set_time(requestParcel.request().time());
				potato::Vector3 from = requestParcel.request().from();
				potato::Vector3 to = requestParcel.request().to();
				notification.set_allocated_from(&from);
				notification.set_allocated_to(&to);
				notification.set_speed(requestParcel.request().speed());
				notification.set_direction(requestParcel.request().direction());
				notification.set_move_id(requestParcel.request().move_id());
				_nerworkServiceProvider.lock()->sendAreacast(session->getSessionId(), _areaRegistry->getArea((*unit)->getAreaId()), torikime::unit::move::Rpc::serializeNotification(notification));
				notification.release_to();
				notification.release_from();
			}
		});

	_rpcBuilder->unit.stop->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
		{
			auto session = weakSession.lock();
			assert(session);

			{
				torikime::unit::stop::Response response;
				response.set_ok(true);
				responser->send(true, std::move(response));
			}

			const auto& units = _unitRegistry->getUnits();
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
				notification.set_area_id((*unit)->getAreaId().value_of());
				notification.set_time(requestParcel.request().time());
				notification.set_stop_time(requestParcel.request().stop_time());
				notification.set_direction(requestParcel.request().direction());
				notification.set_move_id(requestParcel.request().move_id());
				_nerworkServiceProvider.lock()->sendAreacast(session->getSessionId(), _areaRegistry->getArea((*unit)->getAreaId()), torikime::unit::stop::Rpc::serializeNotification(notification));
			}
		});

	{
		_rpcBuilder->battle.skillCast->subscribeRequest([this, weakSession](const auto& requestParcel, auto& responser)
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

				queue.enqueue(0, [this, session, attackId, skillId, triggerTime]()
					{
						const auto& units = _unitRegistry->getUnits();

						std::vector<std::shared_ptr<potato::net::protocol::Payload>> knockbackPayloads;

						{
							auto casterUnit = _unitRegistry->findUnitBySessionId(session->getSessionId());
							fmt::print("attack reveived caster:{} trigger_time:{} skill_id:{} attack_id:{}\n", casterUnit->getUnitId(), triggerTime, skillId, attackId);

							Notification notification;
							notification.set_caster_unit_id(casterUnit->getUnitId().value_of());
							notification.set_trigger_time(triggerTime);
							notification.set_skill_id(skillId);
							notification.set_attack_id(attackId);
							{
								auto casterUnitPositionAtTheTime = casterUnit->getTrackbackPosition(triggerTime);
								for (auto& unit : units)
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
										notification.set_area_id(unit->getAreaId().value_of());
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
										notification.set_area_id(unit->getAreaId().value_of());
										notification.set_time(knockbackCommand->endTime);
										notification.set_stop_time(knockbackCommand->endTime);
										notification.set_direction(knockbackCommand->direction);
										notification.set_move_id(0);
										knockbackPayloads.emplace_back(torikime::unit::stop::Rpc::serializeNotification(notification));
									}
								}
							}
							_nerworkServiceProvider.lock()->sendAreacast(potato::net::SessionId(0), _areaRegistry->getArea(casterUnit->getAreaId()), Rpc::serializeNotification(notification));
							for (auto& payload : knockbackPayloads)
							{
								_nerworkServiceProvider.lock()->sendAreacast(potato::net::SessionId(0), _areaRegistry->getArea(casterUnit->getAreaId()), payload);
							}
						}

					});
			});
	}
}

void GameServiceProvider::onSessionStarted(std::shared_ptr<potato::net::Session>)
{
}

void GameServiceProvider::onDisconnected(std::shared_ptr<potato::net::Session> session)
{
	queue.enqueue(0, [this, session]() {
		// update user id
		auto& session_index = _idMapper.get<potato::net::session_id>();
		auto binderIt = session_index.find(session->getSessionId());
		if (binderIt != session_index.end())
		{
			session_index.replace(binderIt, { binderIt->userId, potato::net::SessionId(0), binderIt->unitId });
			auto user = _userRegistry->find(binderIt->userId);
			user->clearSession();

			const auto now = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
			auto unit = _unitRegistry->findUnitByUnitId(user->getUnitId());
			if (unit)
			{
				unit->onDisconnected(now);
			}
		}
	});
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

void GameServiceProvider::sendAreacastSpawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> spawnUnit)
{
	// broadcast spawn to neighbors
	torikime::unit::spawn::Notification notification;
	notification.set_session_id(sessionId.value_of());
	notification.set_unit_id(spawnUnit->getUnitId().value_of());
	notification.set_area_id(spawnUnit->getAreaId().value_of());
	notification.set_allocated_position(newVector3({ 0, 0, 0 }));
	notification.set_direction(potato::UNIT_DIRECTION_DOWN);
	auto individuality = new potato::Individuality();
	individuality->set_type(potato::UNIT_TYPE_PLAYER);
	notification.set_allocated_individuality(individuality);
	notification.set_cause(potato::UNIT_SPAWN_CAUSE_LOGGEDIN);
	auto avatar = new potato::Avatar();
	avatar->set_name(spawnUnit->getDisplayName());
	notification.set_allocated_avatar(avatar);
	_nerworkServiceProvider.lock()->sendAreacast(sessionId, _areaRegistry->getArea(spawnUnit->getAreaId()), torikime::unit::spawn::Rpc::serializeNotification(notification));
}

void GameServiceProvider::sendSpawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> spawnUnit)
{
	auto area = _areaRegistry->getArea(spawnUnit->getAreaId());
	assert(area);

	auto nerworkServiceProvider = _nerworkServiceProvider.lock();
	assert(nerworkServiceProvider);

	const auto now = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
	area->process([nerworkServiceProvider, now, sessionId, spawnUnit](auto weakUnit)
	{
		auto unit = weakUnit.lock();
		if (unit->getUnitId() == spawnUnit->getUnitId())
		{
			return;
		}

		// spawn
		{
			torikime::unit::spawn::Notification notification;
			notification.set_session_id(unit->getSessionId().value_of());
			notification.set_unit_id(unit->getUnitId().value_of());
			notification.set_area_id(unit->getAreaId().value_of());
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
			nerworkServiceProvider->sendTo(sessionId, payload);
		}

		unit->onSpawn(now);

		// current move state
		{
			auto moveCommand = unit->getLastMoveCommand();
			if (moveCommand != nullptr)
			{
				torikime::unit::move::Notification notification;
				notification.set_unit_id(unit->getUnitId().value_of());
				notification.set_area_id(unit->getAreaId().value_of());
				notification.set_time(moveCommand->startTime);
				notification.set_allocated_from(newVector3(moveCommand->from));
				notification.set_allocated_to(newVector3(moveCommand->to));
				notification.set_speed(moveCommand->speed);
				notification.set_direction(moveCommand->direction);
				notification.set_move_id(moveCommand->moveId);
				nerworkServiceProvider->sendTo(sessionId, torikime::unit::move::Rpc::serializeNotification(notification));
			}
			else
			{
				fmt::print("session[{}] unit[{}] call sendSpawnUnit(Send MoveCommand) but MoveCommand is null\n", sessionId, unit->getUnitId());
				unit->dump(now);
				assert(false);
			}

			auto stopCommand = std::dynamic_pointer_cast<StopCommand>(unit->getLastCommand());
			if (stopCommand == nullptr)
			{
				stopCommand = unit->getFirstStopCommandFromQueue();
			}
			if (stopCommand != nullptr)
			{
				torikime::unit::stop::Notification notification;
				notification.set_unit_id(unit->getUnitId().value_of());
				notification.set_area_id(unit->getAreaId().value_of());
				auto lastMoveCommand = stopCommand->lastMoveCommand.lock();
				auto lastMoveTime = lastMoveCommand != nullptr ? lastMoveCommand->startTime : 0;
				notification.set_time(lastMoveTime);
				notification.set_stop_time(stopCommand->stopTime);
				notification.set_direction(stopCommand->direction);
				notification.set_move_id(stopCommand->moveId);

				auto payload = torikime::unit::stop::Rpc::serializeNotification(notification);
				nerworkServiceProvider->sendTo(sessionId, payload);
			}
			else
			{
				fmt::print("session[{}] unit[{}] call sendSpawnUnit(Send StopCommand) but StopCommand is null\n", sessionId, unit->getUnitId());
				unit->dump(now);
				assert(false);
			}
		}
	});
}

void GameServiceProvider::sendAreacastDespawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> despawnUnit)
{
	if (despawnUnit == nullptr)
	{
		fmt::print("session[{}] call sendDespawn but unit is null\n", sessionId);
		return;
	}
	auto unitDespawn = std::make_shared<torikime::unit::despawn::Rpc>(std::shared_ptr<potato::net::Session>());
	torikime::unit::despawn::Notification notification;
	notification.set_session_id(sessionId.value_of());
	notification.set_unit_id(despawnUnit->getUnitId().value_of());
	notification.set_area_id(despawnUnit->getAreaId().value_of());
	_nerworkServiceProvider.lock()->sendAreacast(sessionId, _areaRegistry->getArea(despawnUnit->getAreaId()), torikime::unit::despawn::Rpc::serializeNotification(notification));

	auto area = _areaRegistry->getArea(despawnUnit->getAreaId());
	area->process([this, sessionId](auto weakUnit)
		{
			auto unit = weakUnit.lock();
			fmt::print("session[{}] call sendDespawn but unit {}\n", sessionId, unit->getUnitId().value_of());
			torikime::unit::despawn::Notification notification;
			notification.set_session_id(unit->getSessionId().value_of());
			notification.set_unit_id(unit->getUnitId().value_of());
			notification.set_area_id(unit->getAreaId().value_of());
			_nerworkServiceProvider.lock()->sendTo(sessionId, torikime::unit::despawn::Rpc::serializeNotification(notification));
		});
}

void GameServiceProvider::sendMove(potato::net::SessionId sessionId, std::shared_ptr<Unit> unit, std::shared_ptr<MoveCommand> moveCommand)
{
	if (moveCommand != nullptr)
	{
		torikime::unit::move::Notification notification;
		notification.set_unit_id(unit->getUnitId().value_of());
		notification.set_area_id(unit->getAreaId().value_of());
		notification.set_time(moveCommand->startTime);
		notification.set_allocated_from(newVector3(moveCommand->from));
		notification.set_allocated_to(newVector3(moveCommand->to));
		notification.set_speed(moveCommand->speed);
		notification.set_direction(moveCommand->direction);
		notification.set_move_id(moveCommand->moveId);
		_nerworkServiceProvider.lock()->sendAreacast(sessionId, _areaRegistry->getArea(unit->getAreaId()), torikime::unit::move::Rpc::serializeNotification(notification));
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
		_nerworkServiceProvider.lock()->sendAreacast(sessionId, _areaRegistry->getArea(unit->getAreaId()), payload);
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
		for (auto& unit : _unitRegistry->getUnits())
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
		ServiceRegistry::instance().getQueue().process();

		sendSystemMessage("hey");

		_userRegistry->update(nowUpdate);
		_areaRegistry->update(nowUpdate);

		{
			fps++;

			const auto now = std::chrono::high_resolution_clock::now();
			if (now >= nextSecond)
			{
				auto nerworkServiceProvider = _nerworkServiceProvider.lock();
				fmt::print("fps: {}, microsec/frame: [{:5}] S:{}/R:{}\n",
					fps,
					fmt::join(frameProcessingTime, ","),
					nerworkServiceProvider->getSendCount(),
					nerworkServiceProvider->getReceiveCount());
				frameProcessingTime.clear();
				nerworkServiceProvider->resetCounters();
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

std::default_random_engine& GameServiceProvider::getRandomEngine()
{
	return _randomEngine;
}

std::shared_ptr<potato::AreaRegistry> GameServiceProvider::getAreaRegistry()
{
	return _areaRegistry;
}

void GameServiceProvider::enqueueSynchronizedAction(SynchronizedAction action)
{
	queue.enqueue(0, action);
}

boost::signals2::connection GameServiceProvider::subscribeOnSpawnReadyRequest(OnSpawnReadyRequestDelegate onSpawnReadyRequestRequest)
{
	return _onSpawnReadyRequest.connect(onSpawnReadyRequestRequest);
}

boost::signals2::connection GameServiceProvider::subscribeOnTransportRequest(OnTransportRequestDelegate onTransportRequest)
{
	return _onTransportRequest.connect(onTransportRequest);
}
