#include "status_component.h"

#include <random>

#include "core/configured_eigen.h"

#include "services/game_service_provider.h"
#include "units/unit.h"

StatusComponent::StatusComponent(std::shared_ptr<GameServiceProvider> gameServiceProvider) : _gameServiceProvider(gameServiceProvider)
{
}

StatusComponent::~StatusComponent() {}

void StatusComponent::update(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/)
{
}
