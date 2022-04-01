#pragma once

#include "units/components/component_types.h"

class GameServiceProvider;

class NpcComponent : public IComponent
{
public:
	NpcComponent(std::shared_ptr<GameServiceProvider> gameServiceProvider);
	virtual ~NpcComponent();

	void update(std::shared_ptr<Unit> unit, int64_t now);

private:
	std::weak_ptr<GameServiceProvider> _gameServiceProvider;
	int64_t _timeScattering = 0;
};
