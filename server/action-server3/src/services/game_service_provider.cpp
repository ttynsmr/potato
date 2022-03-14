#include "game_service_provider.h"

#include <memory>
#include <fmt/core.h>

#include "session/session.h"
#include "services/network_service_provider.h"

#include "proto/message.pb.h"
#include "proto/chat_send_message.pb.h"
#include "proto/diagnosis_sever_sessions.pb.h"
#include "proto/diagnosis_ping_pong.pb.h"
#include "proto/unit_spawn_ready.pb.h"
#include "proto/unit_spawn.pb.h"
#include "proto/unit_despawn.pb.h"
#include "proto/unit_move.pb.h"
#include "proto/unit_stop.pb.h"
#include "proto/battle_skill_cast.pb.h"

#include "rpc.h"
#include "units/unit.h"
#include "units/unit_registory.h"

#include "area/area.h"

#include "generated/cpp/chat_send_message.h"
#include "generated/cpp/diagnosis_sever_sessions.h"
#include "generated/cpp/diagnosis_ping_pong.h"
#include "generated/cpp/unit_spawn_ready.h"
#include "generated/cpp/unit_spawn.h"
#include "generated/cpp/unit_despawn.h"
#include "generated/cpp/unit_move.h"
#include "generated/cpp/unit_stop.h"
#include "generated/cpp/battle_skill_cast.h"

#define forward_declaration(name) \
namespace name \
{ \
	class Rpc; \
	class Responser; \
	class RequestParcel; \
	class Responser; \
	class Notification; \
} \
//

forward_declaration(torikime::chat::send_message)
forward_declaration(torikime::diagnosis::sever_sessions)
forward_declaration(torikime::diagnosis::ping_pong)
forward_declaration(torikime::unit::spawn_ready)
forward_declaration(torikime::unit::spawn)
forward_declaration(torikime::unit::despawn)

GameServiceProvider::GameServiceProvider(std::shared_ptr<Service> service)
	: _service(service), _unitRegistory(std::make_shared<potato::UnitRegistory>()) {}

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
}

void GameServiceProvider::onAccepted(std::shared_ptr<potato::net::session> session)
{
	auto chat = std::make_shared<torikime::chat::send_message::Rpc>(session);
	std::weak_ptr<torikime::chat::send_message::Rpc> weak_chat = chat;
	chat->subscribeRequest([this, weak_chat, session](const torikime::chat::send_message::RequestParcel& request, std::shared_ptr<torikime::chat::send_message::Responser>& responser)
		{
			//std::cout << "receive RequestParcel\n";
			const auto message = request.request().message();
			torikime::chat::send_message::Response response;
			const int64_t messageId = 0;
			response.set_message_id(messageId);
			responser->send(true, std::move(response));

			torikime::chat::send_message::Notification notification;
			notification.set_message(message);
			notification.set_message_id(messageId);
			notification.set_from(std::to_string(session->getSessionId()));
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
	pingPong->subscribeRequest([this, pingPong, session](const torikime::diagnosis::ping_pong::RequestParcel& requestParcel, std::shared_ptr<torikime::diagnosis::ping_pong::Responser>& responser)
		{
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
	unitSpawnReady->subscribeRequest([this, session](const auto& request, auto& responser)
		{
			auto newUnit = _unitRegistory->createUnit(session->getSessionId());

			// response
			{
				std::shared_ptr<potato::Area> area;
				auto areaId = static_cast<AreaId>(request.request().area_id());
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
				response.set_session_id(session->getSessionId());
				response.set_unit_id(newUnit->getUnitId());
				auto position = new potato::Vector3();
				position->set_x(0);
				position->set_y(0);
				position->set_z(0);
				response.set_allocated_position(new potato::Vector3());
				response.set_direction(potato::UNIT_DIRECTION_DOWN);
				auto individuality = new potato::Individuality();
				individuality->set_type(potato::UNIT_TYPE_PLAYER);
				response.set_allocated_individuality(individuality);
				response.set_cause(potato::UNIT_SPAWN_CAUSE_LOGGEDIN);
				auto avatar = new potato::Avatar();
				avatar->set_name(std::string("hoge"));
				response.set_allocated_avatar(avatar);
				responser->send(true, std::move(response));
			}

			{
				// broadcast spawn to neighbors
				torikime::unit::spawn::Notification notification;
				notification.set_session_id(session->getSessionId());
				notification.set_unit_id(newUnit->getUnitId());
				notification.set_area_id(newUnit->getAreaId());
				auto position = new potato::Vector3();
				position->set_x(0);
				position->set_y(0);
				position->set_z(0);
				notification.set_allocated_position(new potato::Vector3());
				notification.set_direction(potato::UNIT_DIRECTION_DOWN);
				auto individuality = new potato::Individuality();
				individuality->set_type(potato::UNIT_TYPE_PLAYER);
				notification.set_allocated_individuality(individuality);
				notification.set_cause(potato::UNIT_SPAWN_CAUSE_LOGGEDIN);
				auto avatar = new potato::Avatar();
				avatar->set_name(std::string("hoge"));
				notification.set_allocated_avatar(avatar);
				_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), torikime::unit::spawn::Rpc::serializeNotification(notification));

				for (const auto unit : _unitRegistory->getUnits())
				{
					if (unit->getUnitId() == newUnit->getUnitId())
					{
						continue;
					}
					// spawn
					{
						torikime::unit::spawn::Notification notification;
						notification.set_session_id(unit->getSessionId());
						notification.set_unit_id(unit->getUnitId());
						notification.set_area_id(unit->getAreaId());
						auto position = new potato::Vector3();
						auto& unitPosition = unit->getPosition();
						position->set_x(unitPosition[0]);
						position->set_y(unitPosition[1]);
						position->set_z(unitPosition[2]);
						notification.set_allocated_position(new potato::Vector3());
						notification.set_direction(potato::UNIT_DIRECTION_DOWN);
						auto individuality = new potato::Individuality();
						individuality->set_type(potato::UNIT_TYPE_PLAYER);
						notification.set_allocated_individuality(individuality);
						notification.set_cause(potato::UNIT_SPAWN_CAUSE_ASIS);
						auto avatar = new potato::Avatar();
						avatar->set_name(std::string("hoge"));
						notification.set_allocated_avatar(avatar);
						auto payload = torikime::unit::spawn::Rpc::serializeNotification(notification);
						_nerworkServiceProvider.lock()->sendTo(session->getSessionId(), payload);
					}

					// current move state
					{
						auto moveCommand = unit->getLastMoveCommand();
						if (moveCommand != nullptr)
						{
							torikime::unit::move::Notification notification;
							notification.set_unit_id(unit->getUnitId());
							notification.set_time(moveCommand->startTime);
							auto from = new potato::Vector3();
							from->set_x(moveCommand->from[0]);
							from->set_y(moveCommand->from[1]);
							from->set_z(moveCommand->from[2]);
							auto to = new potato::Vector3();
							to->set_x(moveCommand->to[0]);
							to->set_y(moveCommand->to[1]);
							to->set_z(moveCommand->to[2]);
							notification.set_allocated_from(from);
							notification.set_allocated_to(to);
							notification.set_speed(moveCommand->speed);
							notification.set_direction(moveCommand->direction);
							notification.set_move_id(moveCommand->moveId);
							_nerworkServiceProvider.lock()->sendTo(session->getSessionId(), torikime::unit::move::Rpc::serializeNotification(notification));
						}

						auto stopCommand = std::dynamic_pointer_cast<StopCommand>(unit->getLastCommand());
						if (stopCommand != nullptr)
						{
							torikime::unit::stop::Notification notification;
							notification.set_unit_id(unit->getUnitId());
							auto lastMoveCommand = stopCommand->lastMoveCommand.lock();
							auto lastMoveTime = lastMoveCommand != nullptr ? lastMoveCommand->startTime : 0;
							notification.set_time(lastMoveTime);
							notification.set_stop_time(stopCommand->stopTime);
							notification.set_direction(stopCommand->direction);
							notification.set_move_id(stopCommand->moveId);

							auto payload = torikime::unit::stop::Rpc::serializeNotification(notification);
							_nerworkServiceProvider.lock()->sendTo(session->getSessionId(), payload);
						}
					}
				}
			}
		});
	_nerworkServiceProvider.lock()->registerRpc(unitSpawnReady);

	auto unitMove = std::make_shared<torikime::unit::move::Rpc>(session);
	unitMove->subscribeRequest([this, session](const auto& requestParcel, auto& responser)
		{
			{
				torikime::unit::move::Response response;
				response.set_ok(true);
				responser->send(true, std::move(response));
			}

			const auto& units = _unitRegistory->getUnits();
			auto unit = std::find_if(units.begin(), units.end(), [requestParcel](auto& u) {
				return requestParcel.request().unit_id() == u->getUnitId();
				});
			if (unit != units.end())
			{
				auto moveCommand = std::make_shared<MoveCommand>();
				moveCommand->startTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
				const auto& from = requestParcel.request().from();
				const auto& to = requestParcel.request().to();
				moveCommand->from = { from.x(), from.y(), from.z() };
				moveCommand->to = { to.x(), to.y(), to.z() };
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
	unitStop->subscribeRequest([this, session](const auto& requestParcel, auto& responser)
		{
			{
				torikime::unit::stop::Response response;
				response.set_ok(true);
				responser->send(true, std::move(response));
			}

			const auto& units = _unitRegistory->getUnits();
			auto unit = std::find_if(units.begin(), units.end(), [requestParcel](auto& u) {
				return requestParcel.request().unit_id() == u->getUnitId();
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
		battleSkillCast->subscribeRequest([this, session](const auto& requestParcel, auto& responser)
			{
				using namespace torikime::battle::skill_cast;

				uint64_t attackId = ++_attackId;
				uint32_t skillId = requestParcel.request().skill_id();
				int64_t triggerTime = requestParcel.request().trigger_time();
				Response response;
				response.set_ok(true);
				response.set_attack_id(attackId);
				responser->send(true, std::move(response));

				queue.enqueue(0, [this, session, attackId, skillId, triggerTime]() {
					const auto& units = _unitRegistory->getUnits();
					//auto unit = std::find_if(units.begin(), units.end(), [](auto& u) {
					//	return requestParcel.request().unit_id() == u->getUnitId();
					//	});
					//if (unit != units.end())
					//{
					//	//auto stopCommand = std::make_shared<StopCommand>();
					//	//stopCommand->stopTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
					//	//stopCommand->direction = requestParcel.request().direction();
					//	//stopCommand->moveId = requestParcel.request().move_id();
					//	//(*unit)->inputCommand(stopCommand);
					//}

					{
						auto casterUnit = _unitRegistory->findUnitBySessionId(session->getSessionId());
						fmt::print("attack reveived caster:{} trigger_time:{} skill_id:{} attack_id:{}\n", casterUnit->getUnitId(), triggerTime, skillId, attackId);

						Notification notification;
						notification.set_caster_unit_id(casterUnit->getUnitId());
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
								auto range = 20.0f;
								if ((receiverUnitPositionAtTheTime - casterUnitPositionAtTheTime).squaredNorm() > range * range)
								{
									fmt::print("too far {} > {}\n", (receiverUnitPositionAtTheTime - casterUnitPositionAtTheTime).norm(), range);
									continue;
								}

								fmt::print("cast skill hit {} to {}\n", casterUnit->getUnitId(), unit->getUnitId());
								auto result = notification.add_results();
								result->set_receiver_unit_id(unit->getUnitId());
								result->set_damage(100);
								result->set_heal(0);
								result->set_miss(false);
								result->set_dodged(false);
							}
						}
						// TODO: change to sendAreacast
						_nerworkServiceProvider.lock()->sendBroadcast(0, Rpc::serializeNotification(notification));
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
	auto unit = _unitRegistory->findUnitBySessionId(session->getSessionId());
	auto unitDespawn = std::make_shared<torikime::unit::despawn::Rpc>(session);
	torikime::unit::despawn::Notification notification;
	notification.set_session_id(session->getSessionId());
	notification.set_unit_id(unit->getUnitId());
	_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), torikime::unit::despawn::Rpc::serializeNotification(notification));

	auto areaId = unit->getAreaId();
	auto areaIt = std::find_if(_areas.begin(), _areas.end(), [areaId](auto& area) { return area->getAreaId() == areaId; });
	if (areaIt != _areas.end())
	{
		(*areaIt)->leave(unit);
	}
	_unitRegistory->unregisterUnit(unit);
}

// TODO: move to message service
void GameServiceProvider::sendSystemMessage(const std::string& message)
{
	torikime::chat::send_message::Notification notification;
	notification.set_message(message);
	notification.set_message_id(++messageId);
	notification.set_from("system");
	// session id 0 is system
	_nerworkServiceProvider.lock()->sendBroadcast(0, torikime::chat::send_message::Rpc::serializeNotification(notification));
}

void GameServiceProvider::main()
{
	auto prev = std::chrono::high_resolution_clock::now();
	queue.appendListener(0, [](std::function<void()> f) { f(); });
	while (_running)
	{
		const auto now = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
		for (auto& unit : _unitRegistory->getUnits())
		{
			unit->update(now);
		}

		queue.process();
		_service->getQueue().process();

		sendSystemMessage("hey");

		{
			auto now = std::chrono::high_resolution_clock::now();
			auto spareTime = std::chrono::high_resolution_clock::now() - prev;
			prev = now;
			std::this_thread::sleep_for(std::chrono::milliseconds(std::max(0L, 100 - std::chrono::duration_cast<std::chrono::microseconds>(spareTime).count())));
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
