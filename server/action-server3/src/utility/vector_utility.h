#pragma once

#include "core/configured_eigen.h"

namespace potato
{
	class Vector3;
}

potato::Vector3* newVector3(const Eigen::Vector3f& v);
Eigen::Vector3f toVector3f(const potato::Vector3& v);
