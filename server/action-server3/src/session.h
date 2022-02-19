#include <boost/asio.hpp>

namespace potato::net
{
    class session
        : public std::enable_shared_from_this<session>
    {
    public:
        session(boost::asio::ip::tcp::socket socket);

        std::shared_ptr<session> start();

    private:
        void do_read();

        void do_write(std::size_t length);

        boost::asio::ip::tcp::socket socket_;
        enum { max_length = 1024 };
        char data_[max_length];
    };
}