# AutoBuild_BDD
這個專案剛開始是用來練習Behavior-driven development。簡稱BDD。後來才又加上自動化建置測試的練習。

## BDD ##
.Net界的BDD Framework，大概只有SpecFlow有人關注，但那種運作方式不太符合我的現況。
所以又看了其他幾種框架，才選了LightBDD，也剛好有人提供一個良好的範例，讓我改寫。

https://github.com/LightBDD/LightBDD

https://github.com/LightBDD/LightBDD.Tutorials

## 自動化建置測試 ##
這裏也是選了一下.Net界的自動化建置的框架，看看分數選了個Nuke Build。透過C#寫自動化任務，整合清除、復原、建置、產生資料庫，測試。
關於資料庫的建立，是使用sqllocaldb當測試DB，產生後用DbUp MigrateDB。

https://github.com/nuke-build/nuke

https://github.com/DbUp/DbUp

https://github.com/martincostello/sqllocaldb

## 執行 ##
先確定系統有裝SqlLocalDb，然後執行_build專案，就可以跑完所有的測試，產生如下的報告。
