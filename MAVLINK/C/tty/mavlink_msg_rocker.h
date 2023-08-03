#pragma once
// MESSAGE ROCKER PACKING

#define MAVLINK_MSG_ID_ROCKER 0


typedef struct __mavlink_rocker_t {
 int16_t leftX; /*<  */
 int16_t leftY; /*<  */
 int16_t rightX; /*<  */
 int16_t rightY; /*<  */
 int16_t switchA; /*<  */
 int16_t switchB; /*<  */
 int16_t switchC; /*<  */
 int16_t switchD; /*<  */
 int16_t switchE; /*<  */
 int16_t switchF; /*<  */
 int16_t switchG; /*<  */
} mavlink_rocker_t;

#define MAVLINK_MSG_ID_ROCKER_LEN 22
#define MAVLINK_MSG_ID_ROCKER_MIN_LEN 22
#define MAVLINK_MSG_ID_0_LEN 22
#define MAVLINK_MSG_ID_0_MIN_LEN 22

#define MAVLINK_MSG_ID_ROCKER_CRC 253
#define MAVLINK_MSG_ID_0_CRC 253



#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_ROCKER { \
    0, \
    "ROCKER", \
    11, \
    {  { "leftX", NULL, MAVLINK_TYPE_INT16_T, 0, 0, offsetof(mavlink_rocker_t, leftX) }, \
         { "leftY", NULL, MAVLINK_TYPE_INT16_T, 0, 2, offsetof(mavlink_rocker_t, leftY) }, \
         { "rightX", NULL, MAVLINK_TYPE_INT16_T, 0, 4, offsetof(mavlink_rocker_t, rightX) }, \
         { "rightY", NULL, MAVLINK_TYPE_INT16_T, 0, 6, offsetof(mavlink_rocker_t, rightY) }, \
         { "switchA", NULL, MAVLINK_TYPE_INT16_T, 0, 8, offsetof(mavlink_rocker_t, switchA) }, \
         { "switchB", NULL, MAVLINK_TYPE_INT16_T, 0, 10, offsetof(mavlink_rocker_t, switchB) }, \
         { "switchC", NULL, MAVLINK_TYPE_INT16_T, 0, 12, offsetof(mavlink_rocker_t, switchC) }, \
         { "switchD", NULL, MAVLINK_TYPE_INT16_T, 0, 14, offsetof(mavlink_rocker_t, switchD) }, \
         { "switchE", NULL, MAVLINK_TYPE_INT16_T, 0, 16, offsetof(mavlink_rocker_t, switchE) }, \
         { "switchF", NULL, MAVLINK_TYPE_INT16_T, 0, 18, offsetof(mavlink_rocker_t, switchF) }, \
         { "switchG", NULL, MAVLINK_TYPE_INT16_T, 0, 20, offsetof(mavlink_rocker_t, switchG) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_ROCKER { \
    "ROCKER", \
    11, \
    {  { "leftX", NULL, MAVLINK_TYPE_INT16_T, 0, 0, offsetof(mavlink_rocker_t, leftX) }, \
         { "leftY", NULL, MAVLINK_TYPE_INT16_T, 0, 2, offsetof(mavlink_rocker_t, leftY) }, \
         { "rightX", NULL, MAVLINK_TYPE_INT16_T, 0, 4, offsetof(mavlink_rocker_t, rightX) }, \
         { "rightY", NULL, MAVLINK_TYPE_INT16_T, 0, 6, offsetof(mavlink_rocker_t, rightY) }, \
         { "switchA", NULL, MAVLINK_TYPE_INT16_T, 0, 8, offsetof(mavlink_rocker_t, switchA) }, \
         { "switchB", NULL, MAVLINK_TYPE_INT16_T, 0, 10, offsetof(mavlink_rocker_t, switchB) }, \
         { "switchC", NULL, MAVLINK_TYPE_INT16_T, 0, 12, offsetof(mavlink_rocker_t, switchC) }, \
         { "switchD", NULL, MAVLINK_TYPE_INT16_T, 0, 14, offsetof(mavlink_rocker_t, switchD) }, \
         { "switchE", NULL, MAVLINK_TYPE_INT16_T, 0, 16, offsetof(mavlink_rocker_t, switchE) }, \
         { "switchF", NULL, MAVLINK_TYPE_INT16_T, 0, 18, offsetof(mavlink_rocker_t, switchF) }, \
         { "switchG", NULL, MAVLINK_TYPE_INT16_T, 0, 20, offsetof(mavlink_rocker_t, switchG) }, \
         } \
}
#endif

/**
 * @brief Pack a rocker message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param leftX  
 * @param leftY  
 * @param rightX  
 * @param rightY  
 * @param switchA  
 * @param switchB  
 * @param switchC  
 * @param switchD  
 * @param switchE  
 * @param switchF  
 * @param switchG  
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_rocker_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               int16_t leftX, int16_t leftY, int16_t rightX, int16_t rightY, int16_t switchA, int16_t switchB, int16_t switchC, int16_t switchD, int16_t switchE, int16_t switchF, int16_t switchG)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_ROCKER_LEN];
    _mav_put_int16_t(buf, 0, leftX);
    _mav_put_int16_t(buf, 2, leftY);
    _mav_put_int16_t(buf, 4, rightX);
    _mav_put_int16_t(buf, 6, rightY);
    _mav_put_int16_t(buf, 8, switchA);
    _mav_put_int16_t(buf, 10, switchB);
    _mav_put_int16_t(buf, 12, switchC);
    _mav_put_int16_t(buf, 14, switchD);
    _mav_put_int16_t(buf, 16, switchE);
    _mav_put_int16_t(buf, 18, switchF);
    _mav_put_int16_t(buf, 20, switchG);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_ROCKER_LEN);
#else
    mavlink_rocker_t packet;
    packet.leftX = leftX;
    packet.leftY = leftY;
    packet.rightX = rightX;
    packet.rightY = rightY;
    packet.switchA = switchA;
    packet.switchB = switchB;
    packet.switchC = switchC;
    packet.switchD = switchD;
    packet.switchE = switchE;
    packet.switchF = switchF;
    packet.switchG = switchG;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_ROCKER_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_ROCKER;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_ROCKER_MIN_LEN, MAVLINK_MSG_ID_ROCKER_LEN, MAVLINK_MSG_ID_ROCKER_CRC);
}

/**
 * @brief Pack a rocker message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param leftX  
 * @param leftY  
 * @param rightX  
 * @param rightY  
 * @param switchA  
 * @param switchB  
 * @param switchC  
 * @param switchD  
 * @param switchE  
 * @param switchF  
 * @param switchG  
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_rocker_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   int16_t leftX,int16_t leftY,int16_t rightX,int16_t rightY,int16_t switchA,int16_t switchB,int16_t switchC,int16_t switchD,int16_t switchE,int16_t switchF,int16_t switchG)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_ROCKER_LEN];
    _mav_put_int16_t(buf, 0, leftX);
    _mav_put_int16_t(buf, 2, leftY);
    _mav_put_int16_t(buf, 4, rightX);
    _mav_put_int16_t(buf, 6, rightY);
    _mav_put_int16_t(buf, 8, switchA);
    _mav_put_int16_t(buf, 10, switchB);
    _mav_put_int16_t(buf, 12, switchC);
    _mav_put_int16_t(buf, 14, switchD);
    _mav_put_int16_t(buf, 16, switchE);
    _mav_put_int16_t(buf, 18, switchF);
    _mav_put_int16_t(buf, 20, switchG);

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_ROCKER_LEN);
#else
    mavlink_rocker_t packet;
    packet.leftX = leftX;
    packet.leftY = leftY;
    packet.rightX = rightX;
    packet.rightY = rightY;
    packet.switchA = switchA;
    packet.switchB = switchB;
    packet.switchC = switchC;
    packet.switchD = switchD;
    packet.switchE = switchE;
    packet.switchF = switchF;
    packet.switchG = switchG;

        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_ROCKER_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_ROCKER;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_ROCKER_MIN_LEN, MAVLINK_MSG_ID_ROCKER_LEN, MAVLINK_MSG_ID_ROCKER_CRC);
}

/**
 * @brief Encode a rocker struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param rocker C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_rocker_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_rocker_t* rocker)
{
    return mavlink_msg_rocker_pack(system_id, component_id, msg, rocker->leftX, rocker->leftY, rocker->rightX, rocker->rightY, rocker->switchA, rocker->switchB, rocker->switchC, rocker->switchD, rocker->switchE, rocker->switchF, rocker->switchG);
}

/**
 * @brief Encode a rocker struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param rocker C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_rocker_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_rocker_t* rocker)
{
    return mavlink_msg_rocker_pack_chan(system_id, component_id, chan, msg, rocker->leftX, rocker->leftY, rocker->rightX, rocker->rightY, rocker->switchA, rocker->switchB, rocker->switchC, rocker->switchD, rocker->switchE, rocker->switchF, rocker->switchG);
}

/**
 * @brief Send a rocker message
 * @param chan MAVLink channel to send the message
 *
 * @param leftX  
 * @param leftY  
 * @param rightX  
 * @param rightY  
 * @param switchA  
 * @param switchB  
 * @param switchC  
 * @param switchD  
 * @param switchE  
 * @param switchF  
 * @param switchG  
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_rocker_send(mavlink_channel_t chan, int16_t leftX, int16_t leftY, int16_t rightX, int16_t rightY, int16_t switchA, int16_t switchB, int16_t switchC, int16_t switchD, int16_t switchE, int16_t switchF, int16_t switchG)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_ROCKER_LEN];
    _mav_put_int16_t(buf, 0, leftX);
    _mav_put_int16_t(buf, 2, leftY);
    _mav_put_int16_t(buf, 4, rightX);
    _mav_put_int16_t(buf, 6, rightY);
    _mav_put_int16_t(buf, 8, switchA);
    _mav_put_int16_t(buf, 10, switchB);
    _mav_put_int16_t(buf, 12, switchC);
    _mav_put_int16_t(buf, 14, switchD);
    _mav_put_int16_t(buf, 16, switchE);
    _mav_put_int16_t(buf, 18, switchF);
    _mav_put_int16_t(buf, 20, switchG);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ROCKER, buf, MAVLINK_MSG_ID_ROCKER_MIN_LEN, MAVLINK_MSG_ID_ROCKER_LEN, MAVLINK_MSG_ID_ROCKER_CRC);
#else
    mavlink_rocker_t packet;
    packet.leftX = leftX;
    packet.leftY = leftY;
    packet.rightX = rightX;
    packet.rightY = rightY;
    packet.switchA = switchA;
    packet.switchB = switchB;
    packet.switchC = switchC;
    packet.switchD = switchD;
    packet.switchE = switchE;
    packet.switchF = switchF;
    packet.switchG = switchG;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ROCKER, (const char *)&packet, MAVLINK_MSG_ID_ROCKER_MIN_LEN, MAVLINK_MSG_ID_ROCKER_LEN, MAVLINK_MSG_ID_ROCKER_CRC);
#endif
}

/**
 * @brief Send a rocker message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_rocker_send_struct(mavlink_channel_t chan, const mavlink_rocker_t* rocker)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_rocker_send(chan, rocker->leftX, rocker->leftY, rocker->rightX, rocker->rightY, rocker->switchA, rocker->switchB, rocker->switchC, rocker->switchD, rocker->switchE, rocker->switchF, rocker->switchG);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ROCKER, (const char *)rocker, MAVLINK_MSG_ID_ROCKER_MIN_LEN, MAVLINK_MSG_ID_ROCKER_LEN, MAVLINK_MSG_ID_ROCKER_CRC);
#endif
}

#if MAVLINK_MSG_ID_ROCKER_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This varient of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_rocker_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  int16_t leftX, int16_t leftY, int16_t rightX, int16_t rightY, int16_t switchA, int16_t switchB, int16_t switchC, int16_t switchD, int16_t switchE, int16_t switchF, int16_t switchG)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_int16_t(buf, 0, leftX);
    _mav_put_int16_t(buf, 2, leftY);
    _mav_put_int16_t(buf, 4, rightX);
    _mav_put_int16_t(buf, 6, rightY);
    _mav_put_int16_t(buf, 8, switchA);
    _mav_put_int16_t(buf, 10, switchB);
    _mav_put_int16_t(buf, 12, switchC);
    _mav_put_int16_t(buf, 14, switchD);
    _mav_put_int16_t(buf, 16, switchE);
    _mav_put_int16_t(buf, 18, switchF);
    _mav_put_int16_t(buf, 20, switchG);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ROCKER, buf, MAVLINK_MSG_ID_ROCKER_MIN_LEN, MAVLINK_MSG_ID_ROCKER_LEN, MAVLINK_MSG_ID_ROCKER_CRC);
#else
    mavlink_rocker_t *packet = (mavlink_rocker_t *)msgbuf;
    packet->leftX = leftX;
    packet->leftY = leftY;
    packet->rightX = rightX;
    packet->rightY = rightY;
    packet->switchA = switchA;
    packet->switchB = switchB;
    packet->switchC = switchC;
    packet->switchD = switchD;
    packet->switchE = switchE;
    packet->switchF = switchF;
    packet->switchG = switchG;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ROCKER, (const char *)packet, MAVLINK_MSG_ID_ROCKER_MIN_LEN, MAVLINK_MSG_ID_ROCKER_LEN, MAVLINK_MSG_ID_ROCKER_CRC);
#endif
}
#endif

#endif

// MESSAGE ROCKER UNPACKING


/**
 * @brief Get field leftX from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_leftX(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  0);
}

/**
 * @brief Get field leftY from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_leftY(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  2);
}

/**
 * @brief Get field rightX from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_rightX(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  4);
}

/**
 * @brief Get field rightY from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_rightY(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  6);
}

/**
 * @brief Get field switchA from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_switchA(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  8);
}

/**
 * @brief Get field switchB from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_switchB(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  10);
}

/**
 * @brief Get field switchC from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_switchC(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  12);
}

/**
 * @brief Get field switchD from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_switchD(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  14);
}

/**
 * @brief Get field switchE from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_switchE(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  16);
}

/**
 * @brief Get field switchF from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_switchF(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  18);
}

/**
 * @brief Get field switchG from rocker message
 *
 * @return  
 */
static inline int16_t mavlink_msg_rocker_get_switchG(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  20);
}

/**
 * @brief Decode a rocker message into a struct
 *
 * @param msg The message to decode
 * @param rocker C-struct to decode the message contents into
 */
static inline void mavlink_msg_rocker_decode(const mavlink_message_t* msg, mavlink_rocker_t* rocker)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    rocker->leftX = mavlink_msg_rocker_get_leftX(msg);
    rocker->leftY = mavlink_msg_rocker_get_leftY(msg);
    rocker->rightX = mavlink_msg_rocker_get_rightX(msg);
    rocker->rightY = mavlink_msg_rocker_get_rightY(msg);
    rocker->switchA = mavlink_msg_rocker_get_switchA(msg);
    rocker->switchB = mavlink_msg_rocker_get_switchB(msg);
    rocker->switchC = mavlink_msg_rocker_get_switchC(msg);
    rocker->switchD = mavlink_msg_rocker_get_switchD(msg);
    rocker->switchE = mavlink_msg_rocker_get_switchE(msg);
    rocker->switchF = mavlink_msg_rocker_get_switchF(msg);
    rocker->switchG = mavlink_msg_rocker_get_switchG(msg);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_ROCKER_LEN? msg->len : MAVLINK_MSG_ID_ROCKER_LEN;
        memset(rocker, 0, MAVLINK_MSG_ID_ROCKER_LEN);
    memcpy(rocker, _MAV_PAYLOAD(msg), len);
#endif
}
