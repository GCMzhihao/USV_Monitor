#pragma once
// MESSAGE PARAM_WRITE_ACK PACKING

#define MAVLINK_MSG_ID_PARAM_WRITE_ACK 4


typedef struct __mavlink_param_write_ack_t {
 uint8_t param_id; /*<  参数类型，详见参数定义*/
} mavlink_param_write_ack_t;

#define MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN 1
#define MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN 1
#define MAVLINK_MSG_ID_4_LEN 1
#define MAVLINK_MSG_ID_4_MIN_LEN 1

#define MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC 54
#define MAVLINK_MSG_ID_4_CRC 54



#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_PARAM_WRITE_ACK { \
    4, \
    "PARAM_WRITE_ACK", \
    1, \
    {  { "param_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_param_write_ack_t, param_id) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_PARAM_WRITE_ACK { \
    "PARAM_WRITE_ACK", \
    1, \
    {  { "param_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_param_write_ack_t, param_id) }, \
         } \
}
#endif

/**
 * @brief Pack a param_write_ack message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param param_id  参数类型，详见参数定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_write_ack_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN];
    _mav_put_uint8_t(buf, 0, param_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN);
#else
    mavlink_param_write_ack_t packet;
    packet.param_id = param_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_PARAM_WRITE_ACK;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC);
}

/**
 * @brief Pack a param_write_ack message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_id  参数类型，详见参数定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_write_ack_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN];
    _mav_put_uint8_t(buf, 0, param_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN);
#else
    mavlink_param_write_ack_t packet;
    packet.param_id = param_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_PARAM_WRITE_ACK;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC);
}

/**
 * @brief Encode a param_write_ack struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param param_write_ack C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_param_write_ack_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_param_write_ack_t* param_write_ack)
{
    return mavlink_msg_param_write_ack_pack(system_id, component_id, msg, param_write_ack->param_id);
}

/**
 * @brief Encode a param_write_ack struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_write_ack C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_param_write_ack_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_param_write_ack_t* param_write_ack)
{
    return mavlink_msg_param_write_ack_pack_chan(system_id, component_id, chan, msg, param_write_ack->param_id);
}

/**
 * @brief Send a param_write_ack message
 * @param chan MAVLink channel to send the message
 *
 * @param param_id  参数类型，详见参数定义
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_param_write_ack_send(mavlink_channel_t chan, uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN];
    _mav_put_uint8_t(buf, 0, param_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_WRITE_ACK, buf, MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC);
#else
    mavlink_param_write_ack_t packet;
    packet.param_id = param_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_WRITE_ACK, (const char *)&packet, MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC);
#endif
}

/**
 * @brief Send a param_write_ack message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_param_write_ack_send_struct(mavlink_channel_t chan, const mavlink_param_write_ack_t* param_write_ack)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_param_write_ack_send(chan, param_write_ack->param_id);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_WRITE_ACK, (const char *)param_write_ack, MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC);
#endif
}

#if MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This varient of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_param_write_ack_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  uint8_t param_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_uint8_t(buf, 0, param_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_WRITE_ACK, buf, MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC);
#else
    mavlink_param_write_ack_t *packet = (mavlink_param_write_ack_t *)msgbuf;
    packet->param_id = param_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_WRITE_ACK, (const char *)packet, MAVLINK_MSG_ID_PARAM_WRITE_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN, MAVLINK_MSG_ID_PARAM_WRITE_ACK_CRC);
#endif
}
#endif

#endif

// MESSAGE PARAM_WRITE_ACK UNPACKING


/**
 * @brief Get field param_id from param_write_ack message
 *
 * @return  参数类型，详见参数定义
 */
static inline uint8_t mavlink_msg_param_write_ack_get_param_id(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  0);
}

/**
 * @brief Decode a param_write_ack message into a struct
 *
 * @param msg The message to decode
 * @param param_write_ack C-struct to decode the message contents into
 */
static inline void mavlink_msg_param_write_ack_decode(const mavlink_message_t* msg, mavlink_param_write_ack_t* param_write_ack)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    param_write_ack->param_id = mavlink_msg_param_write_ack_get_param_id(msg);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN? msg->len : MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN;
        memset(param_write_ack, 0, MAVLINK_MSG_ID_PARAM_WRITE_ACK_LEN);
    memcpy(param_write_ack, _MAV_PAYLOAD(msg), len);
#endif
}
