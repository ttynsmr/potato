#pragma once

#include <memory>
#include "service_registry.h"
#include "service_provider.h"

class SerializeServiceProvider : public IServiceProvider, public std::enable_shared_from_this<SerializeServiceProvider>
{
public:
	SerializeServiceProvider();

	bool isRunning() override;

	void start() override;

	void stop() override;

private:
};
