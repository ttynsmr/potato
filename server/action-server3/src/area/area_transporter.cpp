#include "area_transporter.h"

#include "area_types.h"
#include "area/area.h"
#include "units/unit.h"

#include "services/service_registry.h"
#include "services/game_service_provider.h"
#include "services/network_service_provider.h"

#include "rpc/rpc.h"

#include "area_transport.pb.h"
#include "area_transport.h"

using namespace potato;

static uint64_t s_transportId = 0;

void AreaTransporter::transport(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit)
{
	const uint64_t transportId = ++s_transportId;
	auto gameServiceProvider = ServiceRegistry::instance().findServiceProvider<GameServiceProvider>();
	auto networkServiceProvider = ServiceRegistry::instance().findServiceProvider<NetworkServiceProvider>();

	auto sendAreacastSpawnUnit = [gameServiceProvider, toArea, unit]()
	{
		//spawn to next area
		ServiceRegistry::instance().getQueue().enqueue(ServiceProviderType::Game, [gameServiceProvider, toArea, unit]()
		{
			toArea->enter(unit);
			gameServiceProvider->sendAreacastSpawnUnit(unit->getSessionId(), unit);
		});
	};

	auto onSpawnReadyRequest = [sendAreacastSpawnUnit = std::move(sendAreacastSpawnUnit)]()
	{
		//spawn_ready to next area
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
		ServiceRegistry::instance().getQueue().enqueue(ServiceProviderType::Game, [gameServiceProvider, fromArea, unit]()
		{
			gameServiceProvider->sendAreacastDespawnUnit(unit->getSessionId(), unit);
			fromArea->leave(unit);
		});

		unloadCirrentAreaAndLoadNextArea();
	};

	auto onTransportRequestReceived = [sendAreacastDespawnUnit = std::move(sendAreacastDespawnUnit), transportId](uint64_t receivedTransportId)
	{
		if (transportId != receivedTransportId)
		{
			return;
		}
		sendAreacastDespawnUnit();
	};
	
	_transportRequest = gameServiceProvider->subscribeOnTransportRequest(onTransportRequestReceived);
	
	auto sendAreaTransportNotification = [networkServiceProvider, fromArea, toArea, unit, transportId]()
	{
		//add move_to_area Notification
		ServiceRegistry::instance().getQueue().enqueue(ServiceProviderType::Game, [networkServiceProvider, fromArea, toArea, unit, transportId]()
		{
			using namespace torikime::area::transport;
			Notification notification;
			notification.set_transport_id(transportId);
			notification.set_area_id(toArea->getAreaId().value_of());
			notification.set_unit_id(unit->getUnitId().value_of());
			networkServiceProvider->sendTo(unit->getSessionId(), Rpc::serializeNotification(notification));
		});
	};

	sendAreaTransportNotification();
}
