#pragma once

#include <memory>

#include "units/components/component_types.h"

namespace potato
{
	class AreaTransporter;
}

class AreaTransporterComponent : public IComponent
{
public:
	AreaTransporterComponent(std::shared_ptr<potato::AreaTransporter>& areaTransportrt)
		: _areaTransportrt(areaTransportrt) {}
	virtual ~AreaTransporterComponent() {}

private:
	std::shared_ptr<potato::AreaTransporter> _areaTransportrt;
};
