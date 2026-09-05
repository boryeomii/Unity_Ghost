# 👻 Ghost

> **Photon PUN 2 기반 2인 실시간 멀티플레이 경쟁 게임**

두 플레이어가 제한 시간 동안 맵에 생성되는 아이템을 획득하며 점수를 경쟁하는  
**Unity 기반 2인 실시간 멀티플레이 게임**입니다.

---

## 🎮 Game Overview

| 항목 | 내용 |
|---|---|
| 장르 | 2인 실시간 멀티플레이 경쟁 게임 |
| 플레이 인원 | 2명 |
| 엔진 | Unity |
| 언어 | C# |
| Networking | Photon PUN 2 |
| 플랫폼 | Windows |

### 게임 진행

1. Photon Room에 두 플레이어가 입장합니다.
2. Client가 Ready 상태가 되면 MasterClient가 게임을 시작합니다.
3. 제한 시간 동안 맵에 다양한 아이템이 생성됩니다.
4. 플레이어는 아이템을 획득해 점수를 얻거나 상대 점수를 빼앗니다.
5. 제한 시간이 종료되면 양쪽의 점수를 비교하여 승패를 결정합니다.

---

## 🏗 Network Architecture

Photon PUN 2의 **MasterClient-authoritative 구조**로 구현했습니다.

플레이어 입력과 이동은 각 Player Owner가 처리하고,  
두 플레이어에게 동일하게 유지되어야 하는 공유 게임 상태는 MasterClient가 관리합니다.

| Player Owner | MasterClient |
|---|---|
| 플레이어 입력 | 아이템 생성 및 삭제 |
| 캐릭터 이동 | 아이템 획득 처리 |
| 이동 관련 아이템 효과 | 점수 및 게임 상태 관리 |

플레이어 위치는 `PhotonTransformView`를 통해 동기화하며,  
RPC와 `OnPhotonSerializeView`를 이용해 게임 진행에 필요한 상태를 공유합니다.

---

## 🕹 Controls

| Key | Action |
|---|---|
| `W` `A` `S` `D` / 방향키 | 캐릭터 이동 |
| `ESC` | 게임 나가기 |

---

# 🔐 Photon AppId Setup

보안을 위해 이 Repository에는 개인 Photon Realtime AppId를 포함하지 않았습니다.

따라서 프로젝트를 Clone한 뒤 멀티플레이 기능을 실행하려면  
본인의 Photon Realtime AppId를 `PhotonServerSettings`에 설정해야 합니다.

---

# 🔥 Key Technical Challenges

## 1. MasterClient 기반 네트워크 객체 상태 일관성 확보

### Problem

멀티플레이 환경에서 각 Client가 독립적으로 아이템을 생성하거나 삭제하면  
동일한 게임을 실행하면서도 서로 다른 게임 상태를 가질 수 있습니다.
또한 아이템 삭제와 점수 변경을 각 Client에서 독립적으로 처리하면  
동일한 아이템에 대해 서로 다른 결과가 발생할 가능성이 있습니다.

### Solution

공유 게임 상태 변경의 기준을 **MasterClient 하나로 통일**했습니다.
아이템 생성뿐 아니라 점수 변경과 최종 삭제 역시 MasterClient에서 처리하도록 구성했습니다.

---

## 2. Client 아이템 획득 지연 개선

### Problem

Client 플레이어가 아이템에 충돌 시 MasterClient가 해당 충돌 상태를 확인한 이후 아이템을 삭제했습니다.

### Solution

아이템을 획득한 Player Owner가 자신의 충돌을 직접 감지하고  
MasterClient의 최종 Destroy 응답을 기다리는 동안  
획득한 아이템의 Renderer와 Collider를 먼저 비활성화하여 즉각적인 시각 피드백을 제공했습니다.

---

## 3. 아이템 효과와 네트워크 UI 분리

### Problem

아이템 효과 중 이동속도 변경은 해당 Player Owner에게만 적용되어야 하지만,  
어떤 효과를 획득했는지는 상대방 화면에서도 확인할 수 있어야 했습니다.

### Solution

이를 위해 **실제 Gameplay Effect와 UI Feedback의 실행 범위를 분리**했습니다.
상대방의 상태는 시각적으로 공유하면서  
실제 이동 제어는 해당 Player Owner에게만 적용되도록 구성했습니다.

---

## 4. 게임 진행 중 Player 이탈 처리

### Problem

게임이 시작된 이후 새로운 플레이어가 진행 중인 Room에 입장하면  
게임 시작 RPC와 초기 상태를 전달받지 못해 UI 및 게임 상태가 불일치할 수 있었습니다.

### Solution

게임 시작 시 Room을 닫아 중도 참가를 차단했습니다.
또한 2인 게임 특성상 게임 도중 한 플레이어가 퇴장하면  
남은 플레이어 역시 Room에서 나가 Lobby로 복귀하도록 처리했습니다.
