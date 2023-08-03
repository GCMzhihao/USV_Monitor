#pragma once
// MESSAGE CMD_ACK PACKING

#define MAVLINK_MSG_ID_CMD_ACK 6


typedef struct __mavlink_cmd_ack_t {
 uint8_t cmd_id; /*<  命令类型，详见命令定义*/
 uint8_t cmd_ack_id; /*<  命令应答标志，详见命令应答标志定义*/
} mavlink_cmd_ack_t;

#define MAVLINK_MSG_ID_CMD_ACK_LEN 2
#define MAVLINK_MSG_ID_CMD_ACK_MIN_LEN 2
#define MAVLINK_MSG_ID_6_LEN 2
#define MAVLINK_MSG_ID_6_MIN_LEN 2

#define MAVLINK_MSG_ID_CMD_ACK_CRC 251
#define MAVLINK_MSG_ID_6_CRC 251



#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_CMD_ACK { \
    6, \
    "CMD_ACK", \
    2, \
    {  { "cmd_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_cmd_ack_t, cmd_id) }, \
         { "cmd_ack_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 1, offsetof(mavlink_cmd_ack_t, cmd_ack_id) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_CMD_ACK { \
    "CMD_ACK", \
    2, \
    {  { "cmd_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_cmd_ack_t, cmd_id) }, \
         { "cmd_ack_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 1, offsetof(mavlink_cmd_ack_t, cmd_ack_id) }, \
         } \
}
#endif

/**
 * @brief Pack a cmd_ack message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param cmd_id  命令类型，详见命令定义
 * @param cmd_ack_id  命令应答标志，详见命令应答标志定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_cmd_ack_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               uint8_t cmd_id, uint8_t cmd_ack_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_CMD_ACK_LEN];
    _mav_put_uint8_t(buf, 0, cmd_id);
    _mav_put_uint8_t(buf, 1, cmd_ack_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_CMD_ACK_LEN);
#else
    mavlink_cmd_ack_t packet;
    packet.cmd_id = cmd_id;
    packet.cmd_ack_id = cmd_ack_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_CMD_ACK_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_CMD_ACK;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_CMD_ACK_MIN_LEN, MAVLINK_MSG_ID_CMD_ACK_LEN, MAVLINK_MSG_ID_CMD_ACK_CRC);
}

/**
 * @brief Pack a cmd_ack message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param cmd_id  命令类型，详见命令定义
 * @param cmd_ack_id  命令应答标志，详见命令应答标志定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_cmd_ack_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   uint8_t cmd_id,uint8_t cmd_ack_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_CMD_ACK_LEN];
    _mav_put_uint8_t(buf, 0, cmd_id);
    _mav_put_uint8_t(buf, 1, cmd_ack_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_CMD_ACK_LEN);
#else
    mavlink_cmd_ack_t packet;
    packet.cmd_id = cmd_id;
    packet.cmd_ack_id = cmd_ack_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_CMD_ACK_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_CMD_ACK;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_CMD_ACK_MIN_LEN, MAVLINK_MSG_ID_CMD_ACK_LEN, MAVLINK_MSG_ID_CMD_ACK_CRC);
}

/**
 * @brief Encode a cmd_ack struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param cmd_ack C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_cmd_ack_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_cmd_ack_t* cmd_ack)
{
    return mavlink_msg_cmd_ack_pack(system_id, component_id, msg, cmd_ack->cmd_id, cmd_ack->cmd_ack_id);
}

/**
 * @brief Encode a cmd_ack struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param cmd_ack C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_cmd_ack_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_cmd_ack_t* cmd_ack)
{
    return mavlink_msg_cmd_ack_pack_chan(system_id, component_id, chan, msg, cmd_ack->cmd_id, cmd_ack->cmd_ack_id);
}

/**
 * @brief Send a cmd_ack message
 * @param chan MAVLink channel to send the message
 *
 * @param cmd_id  命令类型，详见命令定义
 * @param cmd_ack_id  命令应答标志，详见命令应答标志定义
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_cmd_ack_send(mavlink_channel_t chan, uint8_t cmd_id, uint8_t cmd_ack_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_CMD_ACK_LEN];
    _mav_put_uint8_t(buf, 0, cmd_id);
    _mav_put_uint8_t(buf, 1, cmd_ack_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_ACK, buf, MAVLINK_MSG_ID_CMD_ACK_MIN_LEN, MAVLINK_MSG_ID_CMD_ACK_LEN, MAVLINK_MSG_ID_CMD_ACK_CRC);
#else
    mavlink_cmd_ack_t packet;
    packet.cmd_id = cmd_id;
    packet.cmd_ack_id = cmd_ack_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_ACK, (const char *)&packet, MAVLINK_MSG_ID_CMD_ACK_MIN_LEN, MAVLINK_MSG_ID_CMD_ACK_LEN, MAVLINK_MSG_ID_CMD_ACK_CRC);
#endif
}

/**
 * @brief Send a cmd_ack message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_cmd_ack_send_struct(mavlink_channel_t chan, const mavlink_cmd_ack_t* cmd_ack)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_cmd_ack_send(chan, cmd_ack->cmd_id, cmd_ack->cmd_ack_id);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_ACK, (const char *)cmd_ack, MAVLINK_MSG_ID_CMD_ACK_MIN_LEN, MAVLINK_MSG_ID_CMD_ACK_LEN, MAVLINK_MSG_ID_CMD_ACK_CRC);
#endif
}

#if MAVLINK_MSG_ID_CMD_ACK_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This varient of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_cmd_ack_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  uint8_t cmd_id, uint8_t cmd_ack_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_uint8_t(buf, 0, cmd_id);
    _mav_put_uint8_t(buf, 1, cmd_ack_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_ACK, buf, MAVLINK_MSG_ID_CMD_ACK_MIN_LEN, MAVLINK_MSG_ID_CMD_ACK_LEN, MAVLINK_MSG_ID_CMD_ACK_CRC);
#else
    mavlink_cmd_ack_t *packet = (mavlink_cmd_ack_t *)msgbuf;
    packet->cmd_id = cmd_id;
    packet->cmd_ack_id = cmd_ack_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_ACK, (const char *)packet, MAVLINK_MSG_ID_CMD_ACK_MIN_LEN, MAVLINK_MSG_ID_CMD_ACK_LEN, MAVLINK_MSG_ID_CMD_ACK_CRC);
#endif
}
#endif

#endif

// MESSAGE CMD_ACK UNPACKING


/**
 * @brief Get field cmd_id from cmd_ack message
 *
 * @return  命令类型，详见命令定义
 */
static inline uint8_t mavlink_msg_cmd_ack_get_cmd_id(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  0);
}

/**
 * @brief Get field cmd_ack_id from cmd_ack message
 *
 * @return  命令应答标志，详见命令应答标志定义
 */
static inline uint8_t mavlink_msg_cmd_ack_get_cmd_ack_id(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  1);
}

/**
 * @brief Decode a cmd_ack message into a struct
 *
 * @param msg The message to decode
 * @param cmd_ack C-struct to decode the message contents into
 */
static inline void mavlink_msg_cmd_ack_decode(const mavlink_message_t* msg, mavlink_cmd_ack_t* cmd_ack)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    cmd_ack->cmd_id = mavlink_msg_cmd_ack_get_cmd_id(msg);
    cmd_ack->cmd_ack_id = mavlink_msg_cmd_ack_get_cmd_ack_id(msg);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_CMD_ACK_LEN? msg->len : MAVLINK_MSG_ID_CMD_ACK_LEN;
        memset(cmd_ack, 0, MAVLINK_MSG_ID_CMD_ACK_LEN);
    memcpy(cmd_ack, _MAV_PAYLOAD(msg), len);
#endif
}
