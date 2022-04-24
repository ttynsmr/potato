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
	AreaTransporterComponent(std::shared_ptr<potato::AreaTransporter>& areaTransporter)
		: _areaTransporter(areaTransporter) {}
	virtual ~AreaTransporterComponent() {}

private:
	std::shared_ptr<potato::AreaTransporter> _areaTransporter;
};
