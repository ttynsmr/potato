#include <iostream>

#include "src/services/service_registry.h"
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

		auto network = ServiceRegistry::instance().registerServiceProvider(std::make_shared<NetworkServiceProvider>(portNumber));
		auto game = ServiceRegistry::instance().registerServiceProvider(std::make_shared<GameServiceProvider>());
		auto serialize = ServiceRegistry::instance().registerServiceProvider(std::make_shared<SerializeServiceProvider>());

		network->start();
		game->start();
		serialize->start();
		ServiceRegistry::instance().run();
	}
	catch (std::exception &e)
	{
		std::cerr << "Exception: " << e.what() << "\n";
	}

	return 0;
}
