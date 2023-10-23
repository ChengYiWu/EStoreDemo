# EStoreDemo 專案

## 簡介
一個簡易的電子購物網站後端 API，其目的為練習採用 .NET 6 從無到有建立專案並能實際有效因應未來需求開發，預計將包含下列功能
1. 商品管理
1. 訂單管理
1. 使用者管理
1. 優惠券管理

## 框架與套件
* .NET 6.0
* EF Core 7
* Azure Storage Blob
* MediatR
* FluentValidation
* Dapper
* Serilog

## 專案架構
採用 Clean Architecture 及 CQRS 架構，並以 Code First 方式進行開發，其中包含下列專案
* Application 
* Domain 
* Infrastructure 
* WebAPI

## 使用者情境

### 商品管理
1. `管理者` 可新增商品與多筆商品規格
1. `管理者` 可修改商品與商品規格
1. `管理者` 可刪除商品或商品規格
1. `管理者` 可查詢商品清單

### 訂單管理
1. `管理者` 可模擬 `顧客` 下訂商品訂單，其中可包含多個商品或商品規格，並可套用優惠券
1. `管理者` 可查詢商品訂單
1. `管理者` 可取消商品訂單
1. `管理者` 可變更商品訂單狀態
    1. 變更至 [ 備貨中 ] 
	1. 變更至 [ 已出貨 ]
	1. 變更至 [ 已取消 ] 
1. `管理者` 可模擬 `顧客` 已取貨操作

### 使用者管理
1. `管理者` 可建立 `顧客` 資料
1. `管理者` 可查詢顧客清單

### 優惠券管理
1. `管理者` 可新增優惠券，包括可採用之商品、可發放數量、折扣金額的相關資訊
1. `管理者` 可查詢優惠券清單
