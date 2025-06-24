# 🎷 Jazz Devil 😈

> **1920년대 러버호스 스타일의 2.5D 리듬 액션 뱀서라이크 Unity 게임**

![Unity](https://img.shields.io/badge/Unity-6000.0.43f1-black?logo=unity&logoColor=white)
![FMOD](https://img.shields.io/badge/FMOD-Audio-red)
![C#](https://img.shields.io/badge/C%23-Programming-blue?logo=c-sharp&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)
![Version](https://img.shields.io/badge/Version-v1.0-orange)

<div align="center">
  <img src="https://via.placeholder.com/800x400/1a1a1a/ffffff?text=🎷+Jazz+Devil+😈+Game+Screenshot" alt="Jazz Devil 게임 스크린샷">
  
  **[📖 사용자 가이드](USER_GUIDE.md) | [🛠️ 개발자 가이드](DEV_GUIDE.md) | [🎮 플레이 데모](#-플레이-데모) | [📺 개발 과정](#-개발-과정)**
</div>

---

## 📜 목차
1. [🎯 프로젝트 소개](#-프로젝트-소개)
2. [✨ 주요 기능](#-주요-기능)
3. [🎮 플레이 데모](#-플레이-데모)
4. [🛠️ 기술 스택](#️-기술-스택)
5. [📁 프로젝트 구조](#-프로젝트-구조)
6. [🚀 실행 방법](#-실행-방법)
7. [👥 팀원 소개](#-팀원-소개)
8. [📺 개발 과정](#-개발-과정)
9. [🗺️ 개발 로드맵](#️-개발-로드맵)
10. [🤝 기여 방법](#-기여-방법)
11. [📄 라이선스](#-라이선스)

<br>

## 🎯 프로젝트 소개

**Jazz Devil**은 1920년대 금주법 시대 LA를 배경으로 한 혁신적인 리듬 액션 게임입니다. 

### 🌟 게임의 특별함

- **🎵 리듬 액션 기반 뱀서라이크**: 뱀파이어 서바이벌의 액션과 리듬 게임의 타이밍이 완벽하게 융합
- **🎨 러버호스 애니메이션**: 증기선 윌리, 뽀빠이 스타일의 매력적인 흑백 애니메이션
- **🎺 재즈 시대 몰입**: 빅밴드 재즈와 함께하는 1920년대 LA 센트럴 애비뉴 경험
- **🤝 동료 연주자 시스템**: 트럼펫, 피아노, 색소폰 연주자들과 함께하는 합주 전투

### 📖 스토리

박자감이 형편없던 멕시코계 드러머 **"토르투가"**가 악마와 계약을 맺어 완벽한 리듬을 얻은 후, 재즈 클럽을 장악하려는 마피아에 맞서 싸우는 이야기를 경험하세요.

**개발 기간**: 2025.04.08 ~ 현재 진행 중

<br>

## ✨ 주요 기능

### 🎵 **리듬 기반 전투 시스템**
- **BPM 동기화**: BGM의 정확한 비트에 맞춰 SPACE를 눌러 공격
- **자동 타겟팅**: 가장 가까운 적을 자동으로 조준하는 직관적인 전투
- **타이밍 판정**: Perfect/Good/Miss에 따른 차별화된 데미지

### 🔥 **콤보 단계 시스템**
- **4단계 콤보**: 연속 성공 시 BPM과 공격력이 점진적으로 상승
- **음악 진화**: 콤보 단계에 따라 빅밴드 악기가 하나씩 추가
- **시각적 피드백**: 콤보 단계별 화려한 이펙트와 화면 연출

### 🌊 **웨이브 시스템**
- **8단계 웨이브**: 점진적으로 증가하는 난이도 (총 플레이 타임 약 10분)
- **다양한 적**: 근접/원거리/보스 몬스터의 전략적 조합
- **중간 상점**: 웨이브 간 전략적 업그레이드와 체력 회복

### 🎺 **동료 연주자 시스템**
- **3가지 악기**: 트럼펫(공격력), 피아노(방어력), 색소폰(속도) 전문
- **시너지 효과**: 동료 조합에 따른 특별한 보너스
- **최대 5명**: 풀 빅밴드 구성으로 합주하는 듯한 전투 경험

### 🛒 **상점 & 성장 시스템**
- **골드 수집**: 몬스터 처치 시 획득하는 전략적 자원
- **다양한 업그레이드**: 공격력, 체력, 특수 능력 강화
- **선택의 재미**: 매 웨이브마다 다른 전략적 선택

<br>

## 🎮 플레이 데모

<div align="center">
  
### 🎥 게임플레이 영상
[![Jazz Devil Gameplay](https://via.placeholder.com/600x300/ff6b35/ffffff?text=▶️+게임플레이+영상)](https://www.youtube.com/watch?v=your-video-link)

### 🎮 플레이 가능한 데모
**[💾 Windows 데모 다운로드](https://naver.me/Gq9v50Ov)**

</div>

### 🎯 조작법
| 키 | 기능 |
|---|---|
| `WASD` | 플레이어 이동 |
| `Space` | 리듬에 맞춰 공격 |
| `ESC` | 일시정지/메뉴 |

<br>

## 🛠️ 기술 스택

<div align="center">

| 영역 | 기술 | 버전 |
|------|------|-------|
| **게임 엔진** | ![Unity](https://img.shields.io/badge/Unity-6000.0.43f1-black?logo=unity&logoColor=white) | 6000.0.43f1 |
| **오디오 미들웨어** | ![FMOD](https://img.shields.io/badge/FMOD-Studio-red) | 2.02 |
| **개발 언어** | ![C#](https://img.shields.io/badge/C%23-Programming-blue?logo=c-sharp&logoColor=white) | .NET 6.0 |
| **렌더링 파이프라인** | ![URP](https://img.shields.io/badge/URP-Universal_Render-purple) | Unity URP |
| **애니메이션** | ![DOTween](https://img.shields.io/badge/DOTween-Animation-green) | 1.2.765 |
| **버전 관리** | ![Git](https://img.shields.io/badge/Git-GitHub_Flow-orange?logo=git&logoColor=white) | Git |

</div>

### 🎨 **특별한 기술적 특징**
- **2.5D 렌더링**: 2D 스프라이트(빌보드) + 3D 환경의 독특한 조합
- **Unity 6 최신 기능**: 향상된 렌더링 파이프라인과 성능 최적화
- **FMOD 통합**: 실시간 BPM 추적 및 동적 음악 레이어링
- **툰 셰이딩**: 러버호스 스타일을 위한 커스텀 셰이더
- **오브젝트 풀링**: 최적화된 몬스터 스폰 시스템
- **GPU Resident Drawer**: Unity 6의 향상된 렌더링 성능

<br>

## 📁 프로젝트 구조

<details>
<summary><strong>📂 자세한 폴더 구조 보기</strong></summary>

```
Assets/
├── 📁 Animator (애니메이터 컴포넌트)
│   └── 📁 Enemy
├── 📁 Anims (애니메이션 클립)
│   ├── 📁 Enemy
│   │   ├── 📁 Bird
│   │   ├── 📁 Frog
│   │   └── 📁 Pig
│   └── 📁 Player
├── 📁 Audio (오디오 클립)
│   ├── 📁 120BPM ~ 160BPM (BPM별 음악 트랙)
│   ├── 📁 INTRO(120BPM)
│   └── 📁 SFX
├── 📁 Font
│   ├── 📁 Cuphead_Font
│   └── 📁 wiched-mouse
├── 📁 Plugins (미들웨어)
│   ├── 📁 DOTween
│   └── 📁 FMOD
├── 📁 Prefabs
│   ├── 📁 Effects
│   ├── 📁 Monster
│   ├── 📁 Supporter
│   └── 📁 UI
├── 📁 Scripts (C# 스크립트)
│   ├── 📁 00. Base (핵심 매니저들)
│   │   ├── 📁 GameManager
│   │   ├── 📁 WaveManager
│   │   ├── 📁 AudioManager
│   │   ├── 📁 RhythmManager
│   │   └── 📁 ComboManager
│   ├── 📁 02. Player
│   ├── 📁 03. Monster
│   ├── 📁 04. Supporter
│   ├── 📁 04. UI
│   └── 📁 05. FieldShop
├── 📁 Sprites (스프라이트 에셋)
├── 📁 StreamingAssets (FMOD 관련)
└── 📄 InputSystem_Actions.inputactions
```

</details>

<br>

## 🚀 실행 방법

### 📋 **시스템 요구사항**
- **OS**: Windows 10/11, macOS 12+, Linux
- **Unity**: 6000.0.43f1 이상
- **RAM**: 16GB 이상 권장 (Unity 6 요구사항)
- **저장공간**: 3GB 이상

### 🛠️ **개발 환경 설정**

1. **저장소 클론**
   ```bash
   git clone https://github.com/probablymayb/Project_JazzDevil.git
   cd Project_JazzDevil
   ```

2. **Unity Hub에서 프로젝트 열기**
   - Unity Hub → Projects → Add → JazzDevil 폴더 선택
   - Unity 6000.0.43f1로 열기

3. **FMOD 설정**
   - `StreamingAssets/JazzDevil_FMOD` 폴더 확인
   - FMOD Studio에서 프로젝트 빌드 (필요시)

4. **플레이**
   - Main Scene 열기
   - Unity 에디터에서 Play 버튼 클릭

### 📦 **빌드 방법**
자세한 빌드 가이드는 **[🛠️ 개발자 가이드](DEV_GUIDE.md)**를 참조하세요.

<br>

## 👥 팀원 소개

<div align="center">

| 🧑‍💻 **이민영** | 🧑‍💻 **박성현** | 🧑‍💻 **박경준** |
|:---:|:---:|:---:|
| **팀장** | **팀원** | **팀원** |
| FMOD 기반 코어 시스템<br>Player 개발 | 적, 동료 시스템 개발 | UI, 웨이브 시스템 개발 |
| [![GitHub](https://img.shields.io/badge/GitHub-probablymayb-black?logo=github)](https://github.com/probablymayb) | [![GitHub](https://img.shields.io/badge/GitHub-ParkSeonghyeon2003-black?logo=github)](https://github.com/ParkSeonghyeon2003) | [![GitHub](https://img.shields.io/badge/GitHub-developerPKJ-black?logo=github)](https://github.com/developerPKJ) |

</div>

### 🏆 **팀 역할 분담**
- **이민영**: 리듬 시스템, FMOD 통합, 플레이어 컨트롤러, Git 관리
- **박성현**: AI 시스템, 몬스터 패턴, 동료 시스템, 애니메이션 통합
- **박경준**: UI/UX 디자인, 웨이브 매니저, 상점 시스템, 스크럼 마스터

<br>

## 📺 개발 과정

### 📈 **개발 진행률**
![Progress](https://img.shields.io/badge/Progress-85%25-brightgreen)

| 시스템 | 진행률 | 상태 |
|--------|--------|------|
| 리듬 시스템 | ![100%](https://progress-bar.dev/100) | ✅ 완료 |
| 전투 시스템 | ![95%](https://progress-bar.dev/95) | ✅ 완료 |
| UI 시스템 | ![90%](https://progress-bar.dev/90) | 🔧 폴리싱 |
| 동료 시스템 | ![80%](https://progress-bar.dev/80) | 🔧 개발 중 |
| 사운드 시스템 | ![85%](https://progress-bar.dev/85) | 🔧 폴리싱 |

### 📊 **개발 통계**
- **총 개발 기간**: 3개월
- **커밋 수**: 200+
- **코드 라인**: 15,000+
- **스프린트**: 6개

<br>

## 🗺️ 개발 로드맵

### ✅ **완료된 기능**
- [x] 기본 플레이어 시스템
- [x] 리듬 기반 전투
- [x] 웨이브 시스템
- [x] 기본 UI
- [x] FMOD 통합

### 🔧 **현재 작업 중**
- [ ] 동료 시스템 완성
- [ ] 추가 몬스터 타입
- [ ] 사운드 밸런싱
- [ ] 버그 수정 및 최적화

### 🎯 **향후 계획**
- [ ] 스팀 출시 준비
- [ ] 추가 웨이브 콘텐츠
- [ ] 멀티플레이어 모드
- [ ] 모바일 이식

<br>

## 🤝 기여 방법

Jazz Devil 프로젝트에 기여하고 싶으신가요? 환영합니다! 🎉

### 📝 **기여 가이드라인**
1. 이 저장소를 Fork 합니다
2. Feature 브랜치를 생성합니다 (`git checkout -b feature/AmazingFeature`)
3. 변경사항을 커밋합니다 (`git commit -m 'Add some AmazingFeature'`)
4. 브랜치에 Push 합니다 (`git push origin feature/AmazingFeature`)
5. Pull Request를 생성합니다

### 🐛 **버그 리포트**
[Issues 페이지](https://github.com/probablymayb/Project_JazzDevil/issues)에서 버그를 신고해주세요.

### 💡 **기능 제안**
새로운 아이디어가 있으시다면 [Discussions](https://github.com/probablymayb/Project_JazzDevil/discussions)에서 토론해보세요!

<br>

## 📄 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다. 자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

---

<div align="center">

### 🎷 Jazz Devil에서 1920년대 재즈의 매력을 느껴보세요! 😈

**[📖 사용자 가이드](USER_GUIDE.md)** • **[🛠️ 개발자 가이드](DEV_GUIDE.md)** • **[🎮 지금 플레이하기](#-플레이-데모)**

[![Stars](https://img.shields.io/github/stars/probablymayb/Project_JazzDevil?style=social)](https://github.com/probablymayb/Project_JazzDevil)
[![Forks](https://img.shields.io/github/forks/probablymayb/Project_JazzDevil?style=social)](https://github.com/probablymayb/Project_JazzDevil)
[![Issues](https://img.shields.io/github/issues/probablymayb/Project_JazzDevil)](https://github.com/probablymayb/Project_JazzDevil/issues)

**Made with ❤️ by Team Jazz Devil**

</div>
