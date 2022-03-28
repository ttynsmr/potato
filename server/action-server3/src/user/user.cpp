#include "user/user.h"

#include "session/session.h"

namespace potato
{
	User::User(UserId userId) : _userId(userId) {}

	UserId User::getUserId() const
	{
		return _userId;
	}

	potato::net::SessionId User::getSessionId() const
	{
		auto session = _session.lock();
		if (session)
		{
			return session->getSessionId();
		}
		else
		{
			return potato::net::SessionId(0);
		}
	}

	void User::setSession(std::shared_ptr<potato::net::session> session)
	{
		_session = session;
	}

	UnitId User::getUnitId() const
	{
		return _unitId;
	}

	void User::setUnitId(UnitId unitId)
	{
		_unitId = unitId;
	}
}
