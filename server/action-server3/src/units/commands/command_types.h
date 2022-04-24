#pragma once

enum class CommandType : int32_t
{
	Stop,
	Move,
	KnockBack,
};

class ICommand {
public:
	ICommand() {}
	virtual ~ICommand() {}

	virtual CommandType getCommandType() const = 0;

	virtual bool isExpired(int64_t now) const = 0;

	virtual long getActionTime() const = 0;
};
