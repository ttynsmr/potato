#include <boost/asio.hpp>
#include <queue>

namespace potato::net::protocol
{
    struct Payload;
}

namespace potato::net
{
    using SessionId = std::int32_t;

    class session
        : public std::enable_shared_from_this<session>
    {
    public:
        session(boost::asio::ip::tcp::socket socket, SessionId sessionId);

        std::shared_ptr<session> start();

        void sendPayload(protocol::Payload& payload);

        using ReceivePayloadDelegate = std::function<void(const protocol::Payload& payload)>;
        void subscribeReceivePayload(ReceivePayloadDelegate callback)
        {
            _receivePayloadDelegate = callback;
        }

        using DisconnectDelegate = std::function<void(SessionId sessionId)>;
        void subscribeDisconnect(DisconnectDelegate callback)
        {
            _disconnectDelegate = callback;
        }

        SessionId getSessionId() const { return _sessionId; }

    private:
        void do_read();

        void do_write(std::size_t length);

        boost::asio::ip::tcp::socket _socket;
        SessionId _sessionId;

        enum { max_length = 1024 };
        char data_[max_length];

        boost::asio::streambuf _receive_buffer;

        ReceivePayloadDelegate _receivePayloadDelegate;
        DisconnectDelegate _disconnectDelegate;
    };
}