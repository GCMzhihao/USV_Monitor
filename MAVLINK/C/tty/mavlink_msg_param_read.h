#pragma once
// MESSAGE PARAM_READ PACKING

#define MAVLINK_MSG_ID_PARAM_READ 1


typedef struct __mavlink_param_read_t {
 uint8_t param_id; /*<  参数类型，详见参数定义*/
} mavlink_param_read_t;

#define MAVLINK_MSG_ID_PARAM_READ_LEN 1
#define MAVLINK_MSG_ID_PARAM_READ_MIN_LEN 1
#define MAVLINK_MSG_ID_1_LEN 1
#define MAVLINK_MSG_ID_1_MIN_LEN 1

#define MAVLINK_MSG_ID_PARAM_READ_CRC 119
#define MAVLINK_MSG_ID_1_CRC 119



#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_PARAM_READ { \
    1, \
    "PARAM_READ", \
    1, \
    {  { "param_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_param_read_t, param_id) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_PARAM_READ { \
    "PARAM_READ", \
    1, \
    {  { "param_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_param_read_t, param_id) }, \
         } \
}
#endif

/**
 * @brief Pack a param_read message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param param_id  参数类型，详见参数定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_read_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_READ_LEN];
    _mav_put_uint8_t(buf, 0, param_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_PARAM_READ_LEN);
#else
    mavlink_param_read_t packet;
    packet.param_id = param_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_PARAM_READ_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_PARAM_READ;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_PARAM_READ_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_LEN, MAVLINK_MSG_ID_PARAM_READ_CRC);
}

/**
 * @brief Pack a param_read message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_id  参数类型，详见参数定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_read_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_READ_LEN];
    _mav_put_uint8_t(buf, 0, param_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_PARAM_READ_LEN);
#else
    mavlink_param_read_t packet;
    packet.param_id = param_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_PARAM_READ_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_PARAM_READ;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_PARAM_READ_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_LEN, MAVLINK_MSG_ID_PARAM_READ_CRC);
}

/**
 * @brief Encode a param_read struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param param_read C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_param_read_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_param_read_t* param_read)
{
    return mavlink_msg_param_read_pack(system_id, component_id, msg, param_read->param_id);
}

/**
 * @brief Encode a param_read struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_read C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_param_read_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_param_read_t* param_read)
{
    return mavlink_msg_param_read_pack_chan(system_id, component_id, chan, msg, param_read->param_id);
}

/**
 * @brief Send a param_read message
 * @param chan MAVLink channel to send the message
 *
 * @param param_id  参数类型，详见参数定义
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_param_read_send(mavlink_channel_t chan, uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_READ_LEN];
    _mav_put_uint8_t(buf, 0, param_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ, buf, MAVLINK_MSG_ID_PARAM_READ_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_LEN, MAVLINK_MSG_ID_PARAM_READ_CRC);
#else
    mavlink_param_read_t packet;
    packet.param_id = param_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ, (const char *)&packet, MAVLINK_MSG_ID_PARAM_READ_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_LEN, MAVLINK_MSG_ID_PARAM_READ_CRC);
#endif
}

/**
 * @brief Send a param_read message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_param_read_send_struct(mavlink_channel_t chan, const mavlink_param_read_t* param_read)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_param_read_send(chan, param_read->param_id);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ, (const char *)param_read, MAVLINK_MSG_ID_PARAM_READ_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_LEN, MAVLINK_MSG_ID_PARAM_READ_CRC);
#endif
}

#if MAVLINK_MSG_ID_PARAM_READ_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This varient of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_param_read_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_uint8_t(buf, 0, param_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ, buf, MAVLINK_MSG_ID_PARAM_READ_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_LEN, MAVLINK_MSG_ID_PARAM_READ_CRC);
#else
    mavlink_param_read_t *packet = (mavlink_param_read_t *)msgbuf;
    packet->param_id = param_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ, (const char *)packet, MAVLINK_MSG_ID_PARAM_READ_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_LEN, MAVLINK_MSG_ID_PARAM_READ_CRC);
#endif
}
#endif

#endif

// MESSAGE PARAM_READ UNPACKING


/**
 * @brief Get field param_id from param_read message
 *
 * @return  参数类型，详见参数定义
 */
static inline uint8_t mavlink_msg_param_read_get_param_id(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  0);
}

/**
 * @brief Decode a param_read message into a struct
 *
 * @param msg The message to decode
 * @param param_read C-struct to decode the message contents into
 */
static inline void mavlink_msg_param_read_decode(const mavlink_message_t* msg, mavlink_param_read_t* param_read)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    param_read->param_id = mavlink_msg_param_read_get_param_id(msg);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_PARAM_READ_LEN? msg->len : MAVLINK_MSG_ID_PARAM_READ_LEN;
        memset(param_read, 0, MAVLINK_MSG_ID_PARAM_READ_LEN);
    memcpy(param_read, _MAV_PAYLOAD(msg), len);
#endif
}
