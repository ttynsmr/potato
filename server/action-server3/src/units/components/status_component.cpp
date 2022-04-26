#include "status_component.h"

#include <random>

#include "core/configured_eigen.h"

#include "rpc/rpc.h"
#include "battle_sync_parameters.pb.h"
#include "battle_sync_parameters.h"

#include "services/game_service_provider.h"
#include "services/network_service_provider.h"
#include "units/unit.h"

#include "area/area_registry.h"

StatusComponent::StatusComponent(
	std::shared_ptr<GameServiceProvider> gameServiceProvider,
	std::shared_ptr<NetworkServiceProvider> networkServiceProvider)
	: _gameServiceProvider(gameServiceProvider)
	, _networkServiceProvider(networkServiceProvider)
{
}

StatusComponent::~StatusComponent() {}

void StatusComponent::onSpawn(std::shared_ptr<Unit> unit, int64_t /*now*/)
{
	auto area = _gameServiceProvider.lock()->getAreaRegistry()->getArea(unit->getAreaId());

	using namespace potato::battle::sync_parameters;
	Notification notification;
	notification.set_unit_id(unit->getUnitId().value_of());
	auto characterStatus = new potato::CharacterStatus();
	characterStatus->set_hitpoint(100);
	characterStatus->set_max_hitpoint(100);
	characterStatus->set_level(1);
	characterStatus->set_gold(0);
	characterStatus->set_exp(0);
	characterStatus->set_stamina(100);
	characterStatus->set_max_stamina(100);
	notification.set_allocated_parameters(characterStatus);
	_networkServiceProvider.lock()->sendAreacast(unit->getSessionId(), area, Rpc::serializeNotification(notification));
}
