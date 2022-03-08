#pragma once

class IServiceProvider
{
public:
	IServiceProvider() {}
	virtual ~IServiceProvider() {}

	virtual bool isRunning() = 0;
	virtual void start() = 0;
	virtual void stop() = 0;
};
