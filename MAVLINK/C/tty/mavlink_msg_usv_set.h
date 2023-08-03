#pragma once
// MESSAGE USV_SET PACKING

#define MAVLINK_MSG_ID_USV_SET 8


typedef struct __mavlink_usv_set_t {
 float Speed; /*<  */
 float Heading; /*<  */
 uint8_t SYS_TYPE; /*<  */
 uint8_t DEV_ID; /*<  */
} mavlink_usv_set_t;

#define MAVLINK_MSG_ID_USV_SET_LEN 10
#define MAVLINK_MSG_ID_USV_SET_MIN_LEN 10
#define MAVLINK_MSG_ID_8_LEN 10
#define MAVLINK_MSG_ID_8_MIN_LEN 10

#define MAVLINK_MSG_ID_USV_SET_CRC 174
#define MAVLINK_MSG_ID_8_CRC 174



#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_USV_SET { \
    8, \
    "USV_SET", \
    4, \
    {  { "SYS_TYPE", NULL, MAVLINK_TYPE_UINT8_T, 0, 8, offsetof(mavlink_usv_set_t, SYS_TYPE) }, \
         { "DEV_ID", NULL, MAVLINK_TYPE_UINT8_T, 0, 9, offsetof(mavlink_usv_set_t, DEV_ID) }, \
         { "Speed", NULL, MAVLINK_TYPE_FLOAT, 0, 0, offsetof(mavlink_usv_set_t, Speed) }, \
         { "Heading", NULL, MAVLINK_TYPE_FLOAT, 0, 4, offsetof(mavlink_usv_set_t, Heading) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_USV_SET { \
    "USV_SET", \
    4, \
    {  { "SYS_TYPE", NULL, MAVLINK_TYPE_UINT8_T, 0, 8, offsetof(mavlink_usv_set_t, SYS_TYPE) }, \
         { "DEV_ID", NULL, MAVLINK_TYPE_UINT8_T, 0, 9, offsetof(mavlink_usv_set_t, DEV_ID) }, \
         { "Speed", NULL, MAVLINK_TYPE_FLOAT, 0, 0, offsetof(mavlink_usv_set_t, Speed) }, \
         { "Heading", NULL, MAVLINK_TYPE_FLOAT, 0, 4, offsetof(mavlink_usv_set_t, Heading) }, \
         } \
}
#endif

/**
 * @brief Pack a usv_set message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param SYS_TYPE  
 * @param DEV_ID  
 * @param Speed  
 * @param Heading  
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_usv_set_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               uint8_t SYS_TYPE, uint8_t DEV_ID, float Speed, float Heading)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_USV_SET_LEN];
    _mav_put_float(buf, 0, Speed);
    _mav_put_float(buf, 4, Heading);
    _mav_put_uint8_t(buf, 8, SYS_TYPE);
    _mav_put_uint8_t(buf, 9, DEV_ID);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_USV_SET_LEN);
#else
    mavlink_usv_set_t packet;
    packet.Speed = Speed;
    packet.Heading = Heading;
    packet.SYS_TYPE = SYS_TYPE;
    packet.DEV_ID = DEV_ID;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_USV_SET_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_USV_SET;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_USV_SET_MIN_LEN, MAVLINK_MSG_ID_USV_SET_LEN, MAVLINK_MSG_ID_USV_SET_CRC);
}

/**
 * @brief Pack a usv_set message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param SYS_TYPE  
 * @param DEV_ID  
 * @param Speed  
 * @param Heading  
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_usv_set_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   uint8_t SYS_TYPE,uint8_t DEV_ID,float Speed,float Heading)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_USV_SET_LEN];
    _mav_put_float(buf, 0, Speed);
    _mav_put_float(buf, 4, Heading);
    _mav_put_uint8_t(buf, 8, SYS_TYPE);
    _mav_put_uint8_t(buf, 9, DEV_ID);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_USV_SET_LEN);
#else
    mavlink_usv_set_t packet;
    packet.Speed = Speed;
    packet.Heading = Heading;
    packet.SYS_TYPE = SYS_TYPE;
    packet.DEV_ID = DEV_ID;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_USV_SET_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_USV_SET;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_USV_SET_MIN_LEN, MAVLINK_MSG_ID_USV_SET_LEN, MAVLINK_MSG_ID_USV_SET_CRC);
}

/**
 * @brief Encode a usv_set struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param usv_set C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_usv_set_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_usv_set_t* usv_set)
{
    return mavlink_msg_usv_set_pack(system_id, component_id, msg, usv_set->SYS_TYPE, usv_set->DEV_ID, usv_set->Speed, usv_set->Heading);
}

/**
 * @brief Encode a usv_set struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param usv_set C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_usv_set_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_usv_set_t* usv_set)
{
    return mavlink_msg_usv_set_pack_chan(system_id, component_id, chan, msg, usv_set->SYS_TYPE, usv_set->DEV_ID, usv_set->Speed, usv_set->Heading);
}

/**
 * @brief Send a usv_set message
 * @param chan MAVLink channel to send the message
 *
 * @param SYS_TYPE  
 * @param DEV_ID  
 * @param Speed  
 * @param Heading  
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_usv_set_send(mavlink_channel_t chan, uint8_t SYS_TYPE, uint8_t DEV_ID, float Speed, float Heading)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_USV_SET_LEN];
    _mav_put_float(buf, 0, Speed);
    _mav_put_float(buf, 4, Heading);
    _mav_put_uint8_t(buf, 8, SYS_TYPE);
    _mav_put_uint8_t(buf, 9, DEV_ID);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_USV_SET, buf, MAVLINK_MSG_ID_USV_SET_MIN_LEN, MAVLINK_MSG_ID_USV_SET_LEN, MAVLINK_MSG_ID_USV_SET_CRC);
#else
    mavlink_usv_set_t packet;
    packet.Speed = Speed;
    packet.Heading = Heading;
    packet.SYS_TYPE = SYS_TYPE;
    packet.DEV_ID = DEV_ID;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_USV_SET, (const char *)&packet, MAVLINK_MSG_ID_USV_SET_MIN_LEN, MAVLINK_MSG_ID_USV_SET_LEN, MAVLINK_MSG_ID_USV_SET_CRC);
#endif
}

/**
 * @brief Send a usv_set message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_usv_set_send_struct(mavlink_channel_t chan, const mavlink_usv_set_t* usv_set)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_usv_set_send(chan, usv_set->SYS_TYPE, usv_set->DEV_ID, usv_set->Speed, usv_set->Heading);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_USV_SET, (const char *)usv_set, MAVLINK_MSG_ID_USV_SET_MIN_LEN, MAVLINK_MSG_ID_USV_SET_LEN, MAVLINK_MSG_ID_USV_SET_CRC);
#endif
}

#if MAVLINK_MSG_ID_USV_SET_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This varient of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_usv_set_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  uint8_t SYS_TYPE, uint8_t DEV_ID, float Speed, float Heading)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_float(buf, 0, Speed);
    _mav_put_float(buf, 4, Heading);
    _mav_put_uint8_t(buf, 8, SYS_TYPE);
    _mav_put_uint8_t(buf, 9, DEV_ID);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_USV_SET, buf, MAVLINK_MSG_ID_USV_SET_MIN_LEN, MAVLINK_MSG_ID_USV_SET_LEN, MAVLINK_MSG_ID_USV_SET_CRC);
#else
    mavlink_usv_set_t *packet = (mavlink_usv_set_t *)msgbuf;
    packet->Speed = Speed;
    packet->Heading = Heading;
    packet->SYS_TYPE = SYS_TYPE;
    packet->DEV_ID = DEV_ID;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_USV_SET, (const char *)packet, MAVLINK_MSG_ID_USV_SET_MIN_LEN, MAVLINK_MSG_ID_USV_SET_LEN, MAVLINK_MSG_ID_USV_SET_CRC);
#endif
}
#endif

#endif

// MESSAGE USV_SET UNPACKING


/**
 * @brief Get field SYS_TYPE from usv_set message
 *
 * @return  
 */
static inline uint8_t mavlink_msg_usv_set_get_SYS_TYPE(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  8);
}

/**
 * @brief Get field DEV_ID from usv_set message
 *
 * @return  
 */
static inline uint8_t mavlink_msg_usv_set_get_DEV_ID(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  9);
}

/**
 * @brief Get field Speed from usv_set message
 *
 * @return  
 */
static inline float mavlink_msg_usv_set_get_Speed(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  0);
}

/**
 * @brief Get field Heading from usv_set message
 *
 * @return  
 */
static inline float mavlink_msg_usv_set_get_Heading(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  4);
}

/**
 * @brief Decode a usv_set message into a struct
 *
 * @param msg The message to decode
 * @param usv_set C-struct to decode the message contents into
 */
static inline void mavlink_msg_usv_set_decode(const mavlink_message_t* msg, mavlink_usv_set_t* usv_set)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    usv_set->Speed = mavlink_msg_usv_set_get_Speed(msg);
    usv_set->Heading = mavlink_msg_usv_set_get_Heading(msg);
    usv_set->SYS_TYPE = mavlink_msg_usv_set_get_SYS_TYPE(msg);
    usv_set->DEV_ID = mavlink_msg_usv_set_get_DEV_ID(msg);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_USV_SET_LEN? msg->len : MAVLINK_MSG_ID_USV_SET_LEN;
        memset(usv_set, 0, MAVLINK_MSG_ID_USV_SET_LEN);
    memcpy(usv_set, _MAV_PAYLOAD(msg), len);
#endif
}
