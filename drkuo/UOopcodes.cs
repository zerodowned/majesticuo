using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drkuo
{
    class UOopcodes
    {
        // Packets sent by client and server
        public int MSG_CharMoveACK = 0x22;
        public int MSG_PingMessage = 0x73;

        public int MSG_TargetCursorCommands = 0x6C;
        public int MSG_SendSkills = 0x3A;
        public int MSG_SecureTrading = 0x6F;
        public int MSG_AllNames = 0x98;
        public int MSG_SendSpeach = 0x1C;
        // Packets sent from the server
        public int SMSG_GameServlist = 0xA8;
        public int SMSG_DrawObject = 0x78;
        public int SMSG_SetWeather = 0x65;
        public int SMSG_WornItem = 0x2E;
        public int SMSG_Deleteobject = 0x1D;
        public int SMSG_UpdatePlayer = 0x77;
        public int SMSG_ClientFeatures = 0xB9;
        public int SMSG_GeneralInformation = 0xBF;
        public int SMSG_CharLocAndBody = 0x1B;
        public int SMSG_OverallLightLevel = 0x4F;
        public int SMSG_ObjectInfo = 0x1A;
        public int SMSG_StatusBarInfo = 0x11;    
        public int SMSG_DrawGamePlayer = 0x20;
        public int SMSG_Damage = 0x0B;
        public int SMSG_CharMoveRejection = 0x21;
        public int SMSG_DraggingOfItem = 0x23;
        public int SMSG_DrawContainer = 0x24;
        public int SMSG_AddItemToContainer = 0x25;
        public int SMSG_KickPlayer = 0x26;
        public int SMSG_RejectMoveItemRequest = 0x27;
        public int SMSG_DropItemApproved = 0x29;
        public int SMSG_Blood = 0x2A;
        public int SMSG_MobAttribute = 0x2D;
        public int SMSG_FightOccuring = 0x2F;
        public int SMSG_AttackOK = 0x30;
        public int SMSG_AddmultipleItemsInContainer = 0x3C;
        public int SMSG_PersonalLightLevel = 0x4E;
        public int SMSG_IdleWarning = 0x53;
        public int SMSG_PlaySoundEffect = 0x54;
        public int SMSG_CharacterAnimation = 0x6E;
        public int SMSG_GraphicalEffect = 0x70;
        public int SMSG_OpenBuyWindow = 0x74;
        public int SMSG_OpenDialogBox = 0x7C;
        public int SMSG_OpenPaperdoll = 0x88;
        public int SMSG_MovePlayer = 0x97;
        public int SMSG_SellList = 0x9E;
        public int SMSG_UpdateCurrentHealth = 0xA1;
        public int SMSG_UpdateCurrentMana = 0xA2;
        public int SMSG_UpdateCurrentStamina = 0xA3;
        public int SMSG_AllowRefuseAttack = 0xAA;
        public int SMSG_GumpTextEntryDialog = 0xAB;
        public int SMSG_SendGumpMenuDialog = 0xB0;
        public int SMSG_CliocMessage = 0xC1;
        public int SMSG_LoginDenied = 0x82;
        public int SMSG_ConnectToGameServer = 0x8C;
        public int SMSG_CharList = 0xA9;
        public int SMSG_GameServerList = 0xA8;
        public int SMSG_ServerChat = 0xAE;

        // Packets sent only via client
        public int CMSG_GetPlayerStatus = 0x34;
        public int CMSG_DropItem = 0x08;
        public int CMSG_Loginreq = 0x80;
        public int CMSG_Pathfind = 0x38; // runuo doesnt support this
        public int CMSG_SingleClick = 0x09;
        public int CMSG_DoubleClick = 0x06;
        public int CMSG_PickUpItem = 0x07;
        public int CMSG_DissconnectNotification = 0x01;
        public int CMSG_MoveRequest = 0x02;
        public int CMSG_TalkRequest = 0x03;
        public int CMSG_RequestAttack = 0x05;
        public int CMSG_RequestSkills = 0x12;
        public int CMSG_DropWearitem = 0x13;
        public int CMSG_BuyItems = 0x3B;
        public int CMSG_RequestWarMode = 0x72;
        public int CMSG_ResponseToDialogBox = 0x7D;
        public int CMSG_SellListReply = 0x9F;
        public int CMSG_ClientSPy = 0xA4;
        public int CMSG_GumpTextEntryDialogReply = 0xAC;
        public int CMSG_GumpMenuSelection = 0xAD;
        public int CMSG_SpyOnClient = 0xD9;
        public int CMSG_LoginRequest = 0x80;
        public int CMSG_SelectServer = 0xA0;
        public int CMSG_GameServerLogin = 0x91;
        public int CMSG_LoginChar = 0x5D;
        public int CMSG_ClientVersion = 0xBD;
    }
}
