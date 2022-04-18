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

boost::future<bool> AreaTransporter::transport(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit)
{
	auto gameServiceProvider = ServiceRegistry::instance().findServiceProvider<GameServiceProvider>();
	auto networkServiceProvider = ServiceRegistry::instance().findServiceProvider<NetworkServiceProvider>();
	return boost::async(boost::launch::async,
		[gameServiceProvider = std::move(gameServiceProvider)
		, networkServiceProvider = std::move(networkServiceProvider)
		, fromArea = std::move(fromArea)
		, toArea = std::move(toArea)
		, unit = std::move(unit)]()
	{
		//add move_to_area Notification
		ServiceRegistry::instance().getQueue().enqueue(ServiceProviderType::Game, [networkServiceProvider, fromArea, toArea, unit]()
		{
			using namespace torikime::area::transport;
			Notification notification;
			notification.set_area_id(toArea->getAreaId().value_of());
			notification.set_unit_id(unit->getUnitId().value_of());
			networkServiceProvider->sendTo(unit->getSessionId(), Rpc::serializeNotification(notification));
		});

		//add move_to_area Request

		//despawn from current area
		ServiceRegistry::instance().getQueue().enqueue(ServiceProviderType::Game, [gameServiceProvider, fromArea = std::move(fromArea), unit]()
		{
			gameServiceProvider->sendAreacastDespawnUnit(unit->getSessionId(), unit);
			fromArea->leave(unit);
		});

		//unload current areaand load next area

		//spawn_ready to next area

		//spawn to next area
		ServiceRegistry::instance().getQueue().enqueue(ServiceProviderType::Game, [gameServiceProvider, toArea = std::move(toArea), unit]()
		{
			toArea->enter(unit);
			gameServiceProvider->sendAreacastSpawnUnit(unit->getSessionId(), unit);
		});

		return true;
	});
}

