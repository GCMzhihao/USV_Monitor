#pragma once
// MESSAGE PARAM_READ_ACK PACKING

#define MAVLINK_MSG_ID_PARAM_READ_ACK 2


typedef struct __mavlink_param_read_ack_t {
 float value; /*<  */
 uint8_t param_id; /*<  参数类型，详见参数定义*/
} mavlink_param_read_ack_t;

#define MAVLINK_MSG_ID_PARAM_READ_ACK_LEN 5
#define MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN 5
#define MAVLINK_MSG_ID_2_LEN 5
#define MAVLINK_MSG_ID_2_MIN_LEN 5

#define MAVLINK_MSG_ID_PARAM_READ_ACK_CRC 14
#define MAVLINK_MSG_ID_2_CRC 14



#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_PARAM_READ_ACK { \
    2, \
    "PARAM_READ_ACK", \
    2, \
    {  { "param_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 4, offsetof(mavlink_param_read_ack_t, param_id) }, \
         { "value", NULL, MAVLINK_TYPE_FLOAT, 0, 0, offsetof(mavlink_param_read_ack_t, value) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_PARAM_READ_ACK { \
    "PARAM_READ_ACK", \
    2, \
    {  { "param_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 4, offsetof(mavlink_param_read_ack_t, param_id) }, \
         { "value", NULL, MAVLINK_TYPE_FLOAT, 0, 0, offsetof(mavlink_param_read_ack_t, value) }, \
         } \
}
#endif

/**
 * @brief Pack a param_read_ack message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param param_id  参数类型，详见参数定义
 * @param value  
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_read_ack_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               uint8_t param_id, float value)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_READ_ACK_LEN];
    _mav_put_float(buf, 0, value);
    _mav_put_uint8_t(buf, 4, param_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN);
#else
    mavlink_param_read_ack_t packet;
    packet.value = value;
    packet.param_id = param_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_PARAM_READ_ACK;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_CRC);
}

/**
 * @brief Pack a param_read_ack message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_id  参数类型，详见参数定义
 * @param value  
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_read_ack_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   uint8_t param_id,float value)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_READ_ACK_LEN];
    _mav_put_float(buf, 0, value);
    _mav_put_uint8_t(buf, 4, param_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN);
#else
    mavlink_param_read_ack_t packet;
    packet.value = value;
    packet.param_id = param_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_PARAM_READ_ACK;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_CRC);
}

/**
 * @brief Encode a param_read_ack struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param param_read_ack C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_param_read_ack_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_param_read_ack_t* param_read_ack)
{
    return mavlink_msg_param_read_ack_pack(system_id, component_id, msg, param_read_ack->param_id, param_read_ack->value);
}

/**
 * @brief Encode a param_read_ack struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_read_ack C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_param_read_ack_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_param_read_ack_t* param_read_ack)
{
    return mavlink_msg_param_read_ack_pack_chan(system_id, component_id, chan, msg, param_read_ack->param_id, param_read_ack->value);
}

/**
 * @brief Send a param_read_ack message
 * @param chan MAVLink channel to send the message
 *
 * @param param_id  参数类型，详见参数定义
 * @param value  
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_param_read_ack_send(mavlink_channel_t chan, uint8_t param_id, float value)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_PARAM_READ_ACK_LEN];
    _mav_put_float(buf, 0, value);
    _mav_put_uint8_t(buf, 4, param_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ_ACK, buf, MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_CRC);
#else
    mavlink_param_read_ack_t packet;
    packet.value = value;
    packet.param_id = param_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ_ACK, (const char *)&packet, MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_CRC);
#endif
}

/**
 * @brief Send a param_read_ack message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_param_read_ack_send_struct(mavlink_channel_t chan, const mavlink_param_read_ack_t* param_read_ack)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_param_read_ack_send(chan, param_read_ack->param_id, param_read_ack->value);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ_ACK, (const char *)param_read_ack, MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_CRC);
#endif
}

#if MAVLINK_MSG_ID_PARAM_READ_ACK_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This varient of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_param_read_ack_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  uint8_t param_id, float value)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_float(buf, 0, value);
    _mav_put_uint8_t(buf, 4, param_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ_ACK, buf, MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_CRC);
#else
    mavlink_param_read_ack_t *packet = (mavlink_param_read_ack_t *)msgbuf;
    packet->value = value;
    packet->param_id = param_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_PARAM_READ_ACK, (const char *)packet, MAVLINK_MSG_ID_PARAM_READ_ACK_MIN_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN, MAVLINK_MSG_ID_PARAM_READ_ACK_CRC);
#endif
}
#endif

#endif

// MESSAGE PARAM_READ_ACK UNPACKING


/**
 * @brief Get field param_id from param_read_ack message
 *
 * @return  参数类型，详见参数定义
 */
static inline uint8_t mavlink_msg_param_read_ack_get_param_id(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  4);
}

/**
 * @brief Get field value from param_read_ack message
 *
 * @return  
 */
static inline float mavlink_msg_param_read_ack_get_value(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  0);
}

/**
 * @brief Decode a param_read_ack message into a struct
 *
 * @param msg The message to decode
 * @param param_read_ack C-struct to decode the message contents into
 */
static inline void mavlink_msg_param_read_ack_decode(const mavlink_message_t* msg, mavlink_param_read_ack_t* param_read_ack)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    param_read_ack->value = mavlink_msg_param_read_ack_get_value(msg);
    param_read_ack->param_id = mavlink_msg_param_read_ack_get_param_id(msg);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_PARAM_READ_ACK_LEN? msg->len : MAVLINK_MSG_ID_PARAM_READ_ACK_LEN;
        memset(param_read_ack, 0, MAVLINK_MSG_ID_PARAM_READ_ACK_LEN);
    memcpy(param_read_ack, _MAV_PAYLOAD(msg), len);
#endif
}
