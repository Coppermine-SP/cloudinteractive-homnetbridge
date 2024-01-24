# HomNetBridge
이 프로젝트는 구형 LG HomNet 기반 스마트 홈 시스템의 서버, 월패드와 연결 된 네트워크의 패킷 캡쳐를 수행하여

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
- [Configure RPI for packet sniffing](#configure-rpi-for-packet-sniffing)
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

## Configure RPI for packet sniffing
**Raspberry Pi에 최신 버전의 Raspberry Pi OS Lite를 설치하십시오.**

>[!NOTE]
>**최신 버전의 Raspberry Pi OS는 아래 페이지에 있습니다.**
>
> https://www.raspberrypi.com/software/operating-systems/

**아래 명령어로 패키지를 최신 상태로 업데이트하고, wireshark, bridge-utils 패키지를 설치하십시오.**
```bash
sudo apt update
sudo apt upgrade
sudo apt install wireshark
sudo apt install bridge-utils
```
**raspi-config에서 wlan0 인터페이스를 관리 인터페이스로 사용할 수 있도록 네트워크에 연결하고, SSH를 활성화 하십시오.**
```bash
raspi-config
```

**Raspberry Pi에 브릿지에 사용할 2번째 네트워크 인터페이스 장치를 연결하고, ifconfig에서 올바르게 활성화되었는지 확인하십시오.**
```bash
copperminesp@rpisrv:~ $ ifconfig

eth0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 169.254.32.188  netmask 255.255.0.0  broadcast 169.254.255.255
        ether b8:27:eb:fc:2c:fd  txqueuelen 1000  (Ethernet)
        RX packets 711338  bytes 49584478 (47.2 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 30277  bytes 6537597 (6.2 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

eth1: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 169.254.113.38  netmask 255.255.0.0  broadcast 169.254.255.255
        ether 00:13:3b:a1:8d:db  txqueuelen 1000  (Ethernet)
        RX packets 25855  bytes 4666669 (4.4 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 715668  bytes 56815830 (54.1 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

lo: flags=73<UP,LOOPBACK,RUNNING>  mtu 65536
        inet 127.0.0.1  netmask 255.0.0.0
        loop  txqueuelen 1000  (Local Loopback)
        RX packets 8  bytes 740 (740.0 B)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 8  bytes 740 (740.0 B)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

wlan0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 172.10.4.3  netmask 255.255.255.0  broadcast 10.10.4.255
        ether b8:27:eb:a9:79:a8  txqueuelen 1000  (Ethernet)
        RX packets 716232  bytes 439918484 (419.5 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 135659  bytes 32423340 (30.9 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

```

**/etc/network/interfaces/ 파일을 다음과 같이 수정하십시오.**
```bash
sudo nano /etc/network/interfaces
```
```
auto lo br0
iface lo inet loopback

iface eth0 inet manual
iface eth1 inet manual

iface br0 inet manual
bridge_ports eth0 eth1
```

**아래 명령어로 브릿지 인터페이스를 생성하십시오.**
```bash
sudo brctl addbr br0
sudo brctl addif br0 eth0 eth1
```

**시스템을 재시작하고 br0 인터페이스가 올바르게 생성되었는지 확인하십시오.**
```bash
sudo reboot
```
```bash
copperminesp@rpisrv:~ $ ifconfig
br0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        ether 00:13:3b:a1:8d:db  txqueuelen 1000  (Ethernet)
        RX packets 736217  bytes 44054738 (42.0 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 2  bytes 108 (108.0 B)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

eth0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        ether b8:27:eb:fc:2c:fd  txqueuelen 1000  (Ethernet)
        RX packets 712053  bytes 49628983 (47.3 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 30296  bytes 6542297 (6.2 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

eth1: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        ether 00:13:3b:a1:8d:db  txqueuelen 1000  (Ethernet)
        RX packets 25867  bytes 4668807 (4.4 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 716390  bytes 56868505 (54.2 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

lo: flags=73<UP,LOOPBACK,RUNNING>  mtu 65536
        inet 127.0.0.1  netmask 255.0.0.0
        loop  txqueuelen 1000  (Local Loopback)
        RX packets 8  bytes 740 (740.0 B)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 8  bytes 740 (740.0 B)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

wlan0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 172.10.4.3  netmask 255.255.255.0  broadcast 10.10.4.255
        ether b8:27:eb:a9:79:a8  txqueuelen 1000  (Ethernet)
        RX packets 717378  bytes 440096837 (419.7 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 136072  bytes 32484026 (30.9 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0
```

