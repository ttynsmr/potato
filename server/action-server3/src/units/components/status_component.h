#pragma once

#include <memory>

#include "units/components/component_types.h"

class GameServiceProvider;
class NetworkServiceProvider;
namespace potato
{
	class Area;
}

class StatusComponent : public IComponent
{
public:
	StatusComponent(std::shared_ptr<GameServiceProvider> gameServiceProvider, std::shared_ptr<NetworkServiceProvider> networkServiceProvider);
	virtual ~StatusComponent();

	void onSpawn(std::shared_ptr<Unit> unit, int64_t now, std::shared_ptr<potato::Area> area);

private:
	std::weak_ptr<GameServiceProvider> _gameServiceProvider;
	std::weak_ptr<NetworkServiceProvider> _networkServiceProvider;
};
