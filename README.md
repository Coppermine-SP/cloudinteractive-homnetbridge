# HomNetBridge
이 프로젝트는 구형 LG HomNet 기반 스마트 홈 시스템에서, 단지 내 서버에서 월패드와 연결 된 네트워크의 패킷 캡쳐를 수행하고,

이를 통해 RS485 통신에서 얻을 수 없는 이벤트 (입차 알림, 공동현관문 알림, 엘리베이터, 에너지)를 Home Assistant를 통해 전파하는 프로젝트입니다.

>[!WARNING]
> **직접적으로 단지 내 서버에 패킷을 전송하지 마십시오.**
>
> 제 3자 장치를 통해 단지 내 서버에 요청을 전송하는 경우, 예기치 못한 오류 또는 법적 문제가 발생 할 수 있습니다.
>
> 이 프로젝트는 네트워크 브릿지를 통해 패킷 캡쳐만 수행하며,
> **네트워크에 대한 직접적인 접근은 수행하지 않습니다.**

### Table of Contents
- [Features](#features)
- [Requirements](#requirements)
- [Configure RPI for packet sniffing]()
- [Install .NET to RPI]()
- [Run project]()

## Features
패킷 캡쳐를 통해 아래의 이벤트를 Home Assistant의 Push 알림을 통해 전달합니다: 
- 입차 알림
- 공동 현관문 출입 요청 알림
- 에너지 리포트 요약
- 엘리베이터 호출시, 엘리베이터 위치 알림
  
## Requirements
- Raspberry Pi (3B+ or better)
- USB Network Adapter (Compatible with RPI)
- Accessibility to CDB (세대통신단자함)
