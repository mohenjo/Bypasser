# Bypasser

![GitHub All Releases](https://img.shields.io/github/downloads/mohenjo/Bypasser/total)

이 응용 프로그램은 SNI(Server Name Indication)를 기반으로 한 HTTPS 필터링의 우회를 시도합니다.

> + 윈도우 `netsh` 커맨드에 대한 UI 버전입니다.
> + 시스템 및 네트워크 인터페이스의 MTU 값을 변경하는 방법으로 우회를 시도합니다.
> + 프로그램의 종료 시 (또는 프로그램이 예기치 못하게 종료되더라도 시스템 재부팅 이후) 시스템 기본값으로 자동 복구됩니다.


## Installation & Usage

### Installation

+ `Download` 폴더 안의 `.zip` 파일을 임의의 폴더에 압축 해제 후 실행하면 됩니다.
+ .NET Framework 설치가 필요할 경우 ([링크](<https://dotnet.microsoft.com/download/dotnet-framework>))에서 .NET Framework 4.6 이상의 런타임을 다운로드합니다.

### Usage

+ 시스템 설정값을 수정하므로, 프로그램 실행 시 관리자 권한을 요청합니다.
  + 프로그램의 종료 또는 재부팅 이후에는 기본값으로 자동 복구됩니다.
+ `Info`-`Network Info` 메뉴에서 현재 시스템의 설정값을 확인할 수 있습니다.
+ 시작과 동시에 MTU 값 변경이 적용되며, 정지 버튼을 클릭하면 기본 MTU 값으로 복구됩니다.


## Project Info

### Version

+ Version 1.19

### Dev Tools

+ [C#](https://docs.microsoft.com/ko-kr/dotnet/csharp/)
+ [Microsoft Visual Studio Community Edition](https://visualstudio.microsoft.com/ko/)

### Environments

+ Test Environment

    + Microsoft Windows 10 (x64)
    + .NET Framework 4.6

+ Dependencies / 3rd-party package(s)

    + None



## License

+ [MIT License](https://github.com/mohenjo/HangulUtils/blob/master/LICENSE)




