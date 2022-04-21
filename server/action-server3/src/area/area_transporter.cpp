#include "area_transporter.h"

#include "area_types.h"
#include "area/area.h"
#include "units/unit.h"
#include "units/components/area_transport_component.h"

#include "services/service_registry.h"
#include "services/game_service_provider.h"
#include "services/network_service_provider.h"

#include "rpc/rpc.h"

#include "area_transport.pb.h"
#include "area_transport.h"

#include "unit_stop.pb.h"
#include "unit_stop.h"

using namespace potato;

static uint64_t s_transportId = 0;

AreaTransporter::AreaTransporter()
{
}

AreaTransporter::~AreaTransporter()
{
	_spawnReadyRequest.disconnect();
	_transportRequest.disconnect();
}

void AreaTransporter::transport(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit, time_t now)
{
	const uint64_t transportId = ++s_transportId;
	auto gameServiceProvider = ServiceRegistry::instance().findServiceProvider<GameServiceProvider>();
	auto networkServiceProvider = ServiceRegistry::instance().findServiceProvider<NetworkServiceProvider>();

	auto sendAreacastSpawnUnit = [gameServiceProvider, toArea, unit]()
	{
		toArea->enter(unit);
		gameServiceProvider->sendAreacastSpawnUnit(unit->getSessionId(), unit);
		unit->removeComponent<AreaTransporterComponent>();
	};

	auto onSpawnReadyRequest = [this, sendAreacastSpawnUnit = std::move(sendAreacastSpawnUnit)]()
	{
		//spawn_ready to next area
		_spawnReadyRequest.disconnect();
		sendAreacastSpawnUnit();
	};

	_spawnReadyRequest = gameServiceProvider->subscribeOnSpawnReadyRequest(onSpawnReadyRequest);

	auto unloadCirrentAreaAndLoadNextArea = []()
	{
		//unload current areaand load next area
	};

	auto sendAreacastDespawnUnit = [unloadCirrentAreaAndLoadNextArea = std::move(unloadCirrentAreaAndLoadNextArea), gameServiceProvider, fromArea, unit]()
	{
		//despawn from current area
		gameServiceProvider->sendAreacastDespawnUnit(unit->getSessionId(), unit);
		fromArea->leave(unit);

		unloadCirrentAreaAndLoadNextArea();
	};

	auto onTransportRequestReceived = [this, sendAreacastDespawnUnit = std::move(sendAreacastDespawnUnit), transportId](uint64_t receivedTransportId)
	{
		if (transportId != receivedTransportId)
		{
			return;
		}
		_transportRequest.disconnect();
		sendAreacastDespawnUnit();
	};
	
	_transportRequest = gameServiceProvider->subscribeOnTransportRequest(onTransportRequestReceived);
	
	auto sendAreaTransportNotification = [networkServiceProvider, fromArea, toArea, unit, now, transportId]()
	{
		//add move_to_area Notification
		{
			using namespace torikime::unit::stop;
			Notification notification;
			notification.set_unit_id(unit->getUnitId().value_of());
			auto lastMoveCommand = unit->getLastMoveCommand();
			auto lastMoveTime = lastMoveCommand != nullptr ? lastMoveCommand->startTime : 0;
			notification.set_time(lastMoveTime);
			notification.set_stop_time(now);
			notification.set_direction(unit->getDirection());
			notification.set_move_id(0);
			networkServiceProvider->sendAreacast(unit->getSessionId(), fromArea, Rpc::serializeNotification(notification));
		}
			
		{
			using namespace torikime::area::transport;
			Notification notification;
			notification.set_transport_id(transportId);
			notification.set_area_id(toArea->getAreaId().value_of());
			notification.set_unit_id(unit->getUnitId().value_of());
			networkServiceProvider->sendTo(unit->getSessionId(), Rpc::serializeNotification(notification));
		}
	};

	sendAreaTransportNotification();
}
