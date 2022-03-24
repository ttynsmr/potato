#include <iostream>

#include "src/services/service.h"
#include "src/services/network_service_provider.h"
#include "src/services/game_service_provider.h"
#include "src/services/serialize_service_provider.h"

int main(int argc, char *argv[])
{
	try
	{
		int32_t portNumber = 28888;
		if (argc != 2)
		{
			std::cerr << "Usage: async_tcp_echo_server <port>\n";
			//return 1;
		}
		else
		{
			portNumber = std::atoi(argv[1]);
		}

		std::shared_ptr<Service> service = std::make_shared<Service>();
		auto network = service->registerServiceProvider(std::make_shared<NetworkServiceProvider>(portNumber, service));
		auto game = service->registerServiceProvider(std::make_shared<GameServiceProvider>(service));
		auto serialize = service->registerServiceProvider(std::make_shared<SerializeServiceProvider>(service));

		network->start();
		game->start();
		serialize->start();
		service->run();
	}
	catch (std::exception &e)
	{
		std::cerr << "Exception: " << e.what() << "\n";
	}

	return 0;
}
