#include <boost/asio.hpp>
#include <queue>

namespace potato::net::protocol
{
    struct Payload;
}

namespace potato::net
{
    class session
        : public std::enable_shared_from_this<session>
    {
    public:
        session(boost::asio::ip::tcp::socket socket);

        std::shared_ptr<session> start();

        void sendPayload(const protocol::Payload& payload);

        using ReceivePayloadDelegate = std::function<void(const protocol::Payload& payload)>;
        void subscribeReceivePayload(ReceivePayloadDelegate callback)
        {
            _receivePayloadDelegate = callback;
        }

    private:
        void do_read();

        void do_write(std::size_t length);

        boost::asio::ip::tcp::socket socket_;
        enum { max_length = 1024 };
        char data_[max_length];

        boost::asio::streambuf receive_buffer_;

        ReceivePayloadDelegate _receivePayloadDelegate;
    };
}