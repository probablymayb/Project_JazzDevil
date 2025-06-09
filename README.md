# 🚀 Project JazzDevil

> 2.5D 리듬 액션 뱀서라이크 Unity 게임

<br>

## 📜 목차
1. [프로젝트 소개](#-프로젝트-소개)
2. [주요 기능](#-주요-기능)
3. [기술 스택](#️-기술-스택)
4. [프로젝트 구조](#-프로젝트-구조)
5. [실행 방법](#-실행-방법)
6. [관련 링크](#-관련-링크)

<br>

## 🧑‍💻 프로젝트 소개

이 프로젝트는 러버호스 그래픽 스타일에 뱀서라이크와 리듬 액션 장르를 혼합한 2.5D 게임을 개발합니다.  
개발 기간: 2025.04.08 ~

### 팀원
| 이름 | 역할 | GitHub |
| --- | --- | --- |
| 이민영 | `팀장`, `FMOD 기반 코어 시스템, Player 개발` | [probablymayb](https://github.com/probablymayb) |
| 박성현 | `팀원`, `적, 동료 시스템 개발` | [ParkSeonghyeon2003](https://github.com/ParkSeonghyeon2003) |
| 박경준 | `팀원`, `UI, 웨이브 시스템 개발` | [developerPKJ](https://github.com/developerPKJ) |

<br>

## ✨ 주요 기능

- **리듬 기반 전투 시스템**: BGM의 비트에 맞게 SPACE를 눌러 공격
- **콤보 단계 시스템**: 타이밍에 맞춰 공격을 연속으로 성공하면 BPM과 공격력이 상승
- **웨이브 시스템**: 플레이어가 웨이브를 생존해 나갈 때 마다 상점 아이템과 함께 더욱 다양한 몬스터들이 스폰
- **상점 / 동료 영입 시스템**: 상점 아이템을 통해 각기 다른 능력을 가진 동료 3명(트럼펫/피아노/색소폰) 중 1명을 영입

<br>

## 🛠️ 기술 스택

- **Game Engine**: <img src="https://img.shields.io/badge/Unity-%23000000.svg?logo=unity&logoColor=white">
- **Sound Engine(Middleware)**: FMOD

<br>

## 📁 프로젝트 구조
```
┌── 📁 Assets (게임의 모든 에셋을 관리) <br>
│   ├── 📁 Animator (애니메이터 컴포넌트) <br>
│   │   └── 📁 Enemy <br>
│   ├── 📁 Anims (애니메이션 클립) <br>
│   │   ├── 📁 Enemy <br>
│   │   │   ├── 📁 Bird <br>
│   │   │   ├── 📁 Frog <br>
│   │   │   └── 📁 Pig <br>
│   │   └── 📁 Player <br>
│   ├── 📁 Audio (오디오 클립) <br>
│   │   ├── 📁 120BPM <br>
│   │   ├── 📁 125BPM <br>
│   │   ├── 📁 130BPM <br>
│   │   ├── 📁 135BPM <br>
│   │   ├── 📁 140BPM <br>
│   │   ├── 📁 145BPM <br>
│   │   ├── 📁 150BPM <br>
│   │   ├── 📁 155BPM <br>
│   │   ├── 📁 160BPM <br>
│   │   ├── 📁 INTRO(120BPM) <br>
│   │   ├── 📁 SFX <br>
│   │   └   └── 📁 Walk <br>
│   ├── 📁 Font <br>
│   │   ├── 📁 Cuphead_Font <br>
│   │   └── 📁 wiched-mouse <br>
│   ├── 📁 Mesh <br>
│   ├── 📁 Plugins (플러그인/미들웨어) <br>
│   │   ├── 📁 Demigiant <br>
│   │   │   └── 📁 DOTween <br>
│   │   └── 📁 FMOD <br>
│   ├── 📁 Prefabs <br>
│   │   ├── 📁 Effects <br>
│   │   ├── 📁 Monster <br>
│   │   ├── 📁 Supporter <br>
│   │   └── 📁 UI <br>
│   ├── 📁 Rendering <br>
│   ├── 📁 Resources (DOTween 세팅) <br>
│   ├── 📁 Scenes <br>
│   ├── 📁 Scriptable Objects <br>
│   │   ├── 📁 00.Monster <br>
│   │   ├── 📁 01.Supporter <br>
│   │   └── 📁 99.Combo <br>
│   ├── 📁 Scripts (C# 스크립트) <br>
│   │   ├── 📁 00. Base <br>
│   │   │   ├── 📁 00. GameManager <br>
│   │   │   ├── 📁 01. WaveManager <br>
│   │   │   ├── 📁 02. AudioManager <br>
│   │   │   ├── 📁 02. SceneManager <br>
│   │   │   ├── 📁 03. RhythmManager <br>
│   │   │   │   └── 📁 00. Note <br>
│   │   │   ├── 📁 04. EventManager <br>
│   │   │   ├── 📁 05. PoolManager <br>
│   │   │   ├── 📁 06. SupporterManager <br>
│   │   │   │── 📁 07. ComboManager <br>
│   │   │   │   ├── 📁 00. ComboData <br>
│   │   │   │   ├── 📁 01. ComboTier <br>
│   │   │   └   └── 📁 02. ComboSkill <br>
│   │   ├── 📁 01. Camera <br>
│   │   ├── 📁 02. Player <br>
│   │   │   └── 📁 00. Shockwave <br>
│   │   ├── 📁 03. Monster <br>
│   │   │   ├── 📁 00. MeleeMonster <br>
│   │   │   ├── 📁 01. RangedMonster <br>
│   │   │   ├── 📁 02. BossMonster <br>
│   │   │   └── 📁 99. MonsterSpawner <br>
│   │   ├── 📁 04. Supporter <br>
│   │   │   ├── 📁 00. Trumpet <br>
│   │   │   ├── 📁 01. Piano <br>
│   │   │   ├── 📁 02. Saxophone <br>
│   │   │   └── 📁 03. KontraBass <br>
│   │   ├── 📁 04. UI <br>
│   │   ├── 📁 05. FieldShop <br>
│   │   └── 📁 99. Utility <br>
│   ├── 📁 Settings <br>
│   ├── 📁 Shaders <br>
│   ├── 📁 Sprites (스프라이트 에셋) <br>
│   │   ├── 📁 Colleague <br>
│   │   ├── 📁 Enemy <br>
│   │   │   ├── 📁 Bird_test <br>
│   │   │   │   ├── 📁 attack <br>
│   │   │   │   ├── 📁 bullet <br>
│   │   │   │   ├── 📁 damaged <br>
│   │   │   │   ├── 📁 idle <br>
│   │   │   │   └── 📁 windup <br>
│   │   │   ├── 📁 Frog <br>
│   │   │   ├── 📁 Frog_test <br>
│   │   │   │   ├── 📁 attack <br>
│   │   │   │   ├── 📁 damaged <br>
│   │   │   │   ├── 📁 idle <br>
│   │   │   │   └── 📁 windup <br>
│   │   │   ├── 📁 Pig_test <br>
│   │   │   │   ├── 📁 attack <br>
│   │   │   │   ├── 📁 damaged <br>
│   │   │   │   ├── 📁 idle <br>
│   │   │   └   └── 📁 windup <br>
│   │   ├── 📁 Map <br>
│   │   ├── 📁 Materials <br>
│   │   ├── 📁 Player <br>
│   │   │   ├── 📁 Player <br>
│   │   │   └── 📁 Player_test (더미) <br>
│   │   └── 📁 UI <br>
│   ├── 📁 StreamingAssets (FMOD 관련 에셋) <br>
│   │   ├── 📁 JazzDevil_FMOD <br>
│   │   │   ├── 📁 .cache <br>
│   │   │   ├── 📁 Assets <br>
│   │   │   └── 📁 Metadata <br>
│   ├── 📄 Billboard.cs (빌보드 렌더링 구현 스크립트) <br>
│   └── 📄 InputSystem_Actions.inputactions (InputSystem) <br>
├── 📁 ProjectSettings <br>
├── 📄 .editorconfig <br>
├── 📄 .gitiattributes <br>
├── 📄 .gitignore <br>
├── 📄 .vsconfig (Visual Studio 설정 파일) <br>
└── 📄 README.md <br>
```
