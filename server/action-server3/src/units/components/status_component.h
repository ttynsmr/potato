#pragma once

#include "units/components/component_types.h"

class GameServiceProvider;

class StatusComponent : public IComponent
{
public:
	StatusComponent(std::shared_ptr<GameServiceProvider> gameServiceProvider);
	virtual ~StatusComponent();

	void update(std::shared_ptr<Unit> unit, int64_t now);

private:
	std::weak_ptr<GameServiceProvider> _gameServiceProvider;
};
