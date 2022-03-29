#pragma once

#include "session/session_types.h"
#include "units/unit_types.h"
#include "user/user_types.h"

#include <boost/multi_index_container.hpp>
#include <boost/multi_index/member.hpp>
#include <boost/multi_index/hashed_index.hpp>

struct IdBinder
{
	UserId userId;
	potato::net::SessionId sessionId;
	UnitId unitId;
};

using IdLookupContainer = boost::multi_index_container<
	IdBinder,
	boost::multi_index::indexed_by<
	boost::multi_index::hashed_unique<boost::multi_index::tag<user_id>, boost::multi_index::member<IdBinder, UserId, &IdBinder::userId> >,
	boost::multi_index::hashed_unique<boost::multi_index::tag<potato::net::session_id>, boost::multi_index::member<IdBinder, potato::net::SessionId, &IdBinder::sessionId> >,
	boost::multi_index::hashed_unique<boost::multi_index::tag<unit_id>, boost::multi_index::member<IdBinder, UnitId, &IdBinder::unitId> >	>
>;

class UserAuthenticator
{
public:
	std::optional<UserId> DoAuth(std::string id, std::string password)
	{
		return std::optional<UserId>(std::hash<std::string>()(id + password));
	}
};

namespace potato
{
	namespace net
	{
		class session;
	}

	class User
	{
	public:
		User(UserId userId);

		UserId getUserId() const;

		potato::net::SessionId getSessionId() const;
		void setSession(std::shared_ptr<potato::net::session> session);
		void clearSession();

		UnitId getUnitId() const;
		void setUnitId(UnitId unitId);

		void update(int64_t now);

		bool isExpired(int64_t now) const;

	private:
		UserId _userId;
		std::weak_ptr<potato::net::session> _session;
		UnitId _unitId;
		int64_t lastConnectedTime = 0;
	};
}
