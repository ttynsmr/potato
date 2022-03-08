#pragma once

#include <memory>
#include "service.h"
#include "service_provider.h"

class SerializeServiceProvider : public IServiceProvider, public std::enable_shared_from_this<SerializeServiceProvider>
{
public:
	SerializeServiceProvider(std::shared_ptr<Service> service);

	bool isRunning();

	void start();

	void stop();

private:
	std::shared_ptr<Service> _service;
};
