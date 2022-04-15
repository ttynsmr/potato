#pragma once

#include <strong_type/strong_type.hpp>
namespace potato
{
	struct area_id {};
	using AreaId = strong::type<int32_t, struct area_id, strong::ordered, strong::equality, strong::hashable, strong::default_constructible>;
}

namespace boost
{
	inline size_t hash_value(const potato::AreaId& v) { return std::hash<potato::AreaId>()(v); }
}

#include <fmt/core.h>
#include <fmt/format.h>
template <> struct fmt::formatter<potato::AreaId> : fmt::formatter<std::string>
{
	template <typename FormatContext>
	auto format(potato::AreaId v, FormatContext& ctx) {
		return fmt::formatter<std::string>::format(fmt::to_string(v.value_of()), ctx);
	}
};
