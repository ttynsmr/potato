#pragma once

#include <strong_type/strong_type.hpp>
struct user_id {};
using UserId = strong::type<uint64_t, struct user_id, strong::ordered, strong::equality, strong::hashable, strong::default_constructible>;

namespace boost
{
	inline size_t hash_value(const UserId& v) { return std::hash<UserId>()(v); }
}

#include <fmt/core.h>
#include <fmt/format.h>
template <> struct fmt::formatter<UserId> : fmt::formatter<std::string>
{
	template <typename FormatContext>
	auto format(UserId v, FormatContext& ctx) {
		return fmt::formatter<std::string>::format(fmt::to_string(v.value_of()), ctx);
	}
};
