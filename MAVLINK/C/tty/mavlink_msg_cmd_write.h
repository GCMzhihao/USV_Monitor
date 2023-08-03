#pragma once
// MESSAGE CMD_WRITE PACKING

#define MAVLINK_MSG_ID_CMD_WRITE 5


typedef struct __mavlink_cmd_write_t {
 uint8_t SYS_TYPE; /*<  */
 uint8_t DEV_ID; /*<  */
 uint8_t cmd_id; /*<  命令类型，详见命令定义*/
} mavlink_cmd_write_t;

#define MAVLINK_MSG_ID_CMD_WRITE_LEN 3
#define MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN 3
#define MAVLINK_MSG_ID_5_LEN 3
#define MAVLINK_MSG_ID_5_MIN_LEN 3

#define MAVLINK_MSG_ID_CMD_WRITE_CRC 220
#define MAVLINK_MSG_ID_5_CRC 220



#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_CMD_WRITE { \
    5, \
    "CMD_WRITE", \
    3, \
    {  { "SYS_TYPE", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_cmd_write_t, SYS_TYPE) }, \
         { "DEV_ID", NULL, MAVLINK_TYPE_UINT8_T, 0, 1, offsetof(mavlink_cmd_write_t, DEV_ID) }, \
         { "cmd_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 2, offsetof(mavlink_cmd_write_t, cmd_id) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_CMD_WRITE { \
    "CMD_WRITE", \
    3, \
    {  { "SYS_TYPE", NULL, MAVLINK_TYPE_UINT8_T, 0, 0, offsetof(mavlink_cmd_write_t, SYS_TYPE) }, \
         { "DEV_ID", NULL, MAVLINK_TYPE_UINT8_T, 0, 1, offsetof(mavlink_cmd_write_t, DEV_ID) }, \
         { "cmd_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 2, offsetof(mavlink_cmd_write_t, cmd_id) }, \
         } \
}
#endif

/**
 * @brief Pack a cmd_write message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param SYS_TYPE  
 * @param DEV_ID  
 * @param cmd_id  命令类型，详见命令定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_cmd_write_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               uint8_t SYS_TYPE, uint8_t DEV_ID, uint8_t cmd_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_CMD_WRITE_LEN];
    _mav_put_uint8_t(buf, 0, SYS_TYPE);
    _mav_put_uint8_t(buf, 1, DEV_ID);
    _mav_put_uint8_t(buf, 2, cmd_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_CMD_WRITE_LEN);
#else
    mavlink_cmd_write_t packet;
    packet.SYS_TYPE = SYS_TYPE;
    packet.DEV_ID = DEV_ID;
    packet.cmd_id = cmd_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_CMD_WRITE_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_CMD_WRITE;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN, MAVLINK_MSG_ID_CMD_WRITE_LEN, MAVLINK_MSG_ID_CMD_WRITE_CRC);
}

/**
 * @brief Pack a cmd_write message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param SYS_TYPE  
 * @param DEV_ID  
 * @param cmd_id  命令类型，详见命令定义
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_cmd_write_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   uint8_t SYS_TYPE,uint8_t DEV_ID,uint8_t cmd_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_CMD_WRITE_LEN];
    _mav_put_uint8_t(buf, 0, SYS_TYPE);
    _mav_put_uint8_t(buf, 1, DEV_ID);
    _mav_put_uint8_t(buf, 2, cmd_id);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_CMD_WRITE_LEN);
#else
    mavlink_cmd_write_t packet;
    packet.SYS_TYPE = SYS_TYPE;
    packet.DEV_ID = DEV_ID;
    packet.cmd_id = cmd_id;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_CMD_WRITE_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_CMD_WRITE;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN, MAVLINK_MSG_ID_CMD_WRITE_LEN, MAVLINK_MSG_ID_CMD_WRITE_CRC);
}

/**
 * @brief Encode a cmd_write struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param cmd_write C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_cmd_write_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_cmd_write_t* cmd_write)
{
    return mavlink_msg_cmd_write_pack(system_id, component_id, msg, cmd_write->SYS_TYPE, cmd_write->DEV_ID, cmd_write->cmd_id);
}

/**
 * @brief Encode a cmd_write struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param cmd_write C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_cmd_write_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_cmd_write_t* cmd_write)
{
    return mavlink_msg_cmd_write_pack_chan(system_id, component_id, chan, msg, cmd_write->SYS_TYPE, cmd_write->DEV_ID, cmd_write->cmd_id);
}

/**
 * @brief Send a cmd_write message
 * @param chan MAVLink channel to send the message
 *
 * @param SYS_TYPE  
 * @param DEV_ID  
 * @param cmd_id  命令类型，详见命令定义
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_cmd_write_send(mavlink_channel_t chan, uint8_t SYS_TYPE, uint8_t DEV_ID, uint8_t cmd_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_CMD_WRITE_LEN];
    _mav_put_uint8_t(buf, 0, SYS_TYPE);
    _mav_put_uint8_t(buf, 1, DEV_ID);
    _mav_put_uint8_t(buf, 2, cmd_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_WRITE, buf, MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN, MAVLINK_MSG_ID_CMD_WRITE_LEN, MAVLINK_MSG_ID_CMD_WRITE_CRC);
#else
    mavlink_cmd_write_t packet;
    packet.SYS_TYPE = SYS_TYPE;
    packet.DEV_ID = DEV_ID;
    packet.cmd_id = cmd_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_WRITE, (const char *)&packet, MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN, MAVLINK_MSG_ID_CMD_WRITE_LEN, MAVLINK_MSG_ID_CMD_WRITE_CRC);
#endif
}

/**
 * @brief Send a cmd_write message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_cmd_write_send_struct(mavlink_channel_t chan, const mavlink_cmd_write_t* cmd_write)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_cmd_write_send(chan, cmd_write->SYS_TYPE, cmd_write->DEV_ID, cmd_write->cmd_id);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_WRITE, (const char *)cmd_write, MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN, MAVLINK_MSG_ID_CMD_WRITE_LEN, MAVLINK_MSG_ID_CMD_WRITE_CRC);
#endif
}

#if MAVLINK_MSG_ID_CMD_WRITE_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This varient of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_cmd_write_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  uint8_t SYS_TYPE, uint8_t DEV_ID, uint8_t cmd_id)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_uint8_t(buf, 0, SYS_TYPE);
    _mav_put_uint8_t(buf, 1, DEV_ID);
    _mav_put_uint8_t(buf, 2, cmd_id);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_WRITE, buf, MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN, MAVLINK_MSG_ID_CMD_WRITE_LEN, MAVLINK_MSG_ID_CMD_WRITE_CRC);
#else
    mavlink_cmd_write_t *packet = (mavlink_cmd_write_t *)msgbuf;
    packet->SYS_TYPE = SYS_TYPE;
    packet->DEV_ID = DEV_ID;
    packet->cmd_id = cmd_id;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_CMD_WRITE, (const char *)packet, MAVLINK_MSG_ID_CMD_WRITE_MIN_LEN, MAVLINK_MSG_ID_CMD_WRITE_LEN, MAVLINK_MSG_ID_CMD_WRITE_CRC);
#endif
}
#endif

#endif

// MESSAGE CMD_WRITE UNPACKING


/**
 * @brief Get field SYS_TYPE from cmd_write message
 *
 * @return  
 */
static inline uint8_t mavlink_msg_cmd_write_get_SYS_TYPE(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  0);
}

/**
 * @brief Get field DEV_ID from cmd_write message
 *
 * @return  
 */
static inline uint8_t mavlink_msg_cmd_write_get_DEV_ID(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  1);
}

/**
 * @brief Get field cmd_id from cmd_write message
 *
 * @return  命令类型，详见命令定义
 */
static inline uint8_t mavlink_msg_cmd_write_get_cmd_id(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  2);
}

/**
 * @brief Decode a cmd_write message into a struct
 *
 * @param msg The message to decode
 * @param cmd_write C-struct to decode the message contents into
 */
static inline void mavlink_msg_cmd_write_decode(const mavlink_message_t* msg, mavlink_cmd_write_t* cmd_write)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    cmd_write->SYS_TYPE = mavlink_msg_cmd_write_get_SYS_TYPE(msg);
    cmd_write->DEV_ID = mavlink_msg_cmd_write_get_DEV_ID(msg);
    cmd_write->cmd_id = mavlink_msg_cmd_write_get_cmd_id(msg);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_CMD_WRITE_LEN? msg->len : MAVLINK_MSG_ID_CMD_WRITE_LEN;
        memset(cmd_write, 0, MAVLINK_MSG_ID_CMD_WRITE_LEN);
    memcpy(cmd_write, _MAV_PAYLOAD(msg), len);
#endif
}
