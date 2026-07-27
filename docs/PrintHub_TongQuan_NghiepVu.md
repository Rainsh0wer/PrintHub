# PrintHub — Tổng quan dự án & Nghiệp vụ

> Tài liệu này trả lời 5 câu hỏi theo đúng thứ tự: **Dự án là gì → Giải quyết vấn đề gì
> → Làm được những gì → Vì sao thiết kế cơ sở dữ liệu như vậy → Nghiệp vụ lấy từ đâu và
> gồm những gì.**
>
> Dùng để ôn trước khi bảo vệ: mỗi mục đều gắn quyết định thiết kế với **lý do nghiệp vụ**,
> không chỉ mô tả tính năng.

---

## Mục lục

1. [Giới thiệu dự án](#1-giới-thiệu-dự-án)
2. [Vấn đề thực tế (Problems)](#2-vấn-đề-thực-tế-problems)
3. [PrintHub giải quyết vấn đề như thế nào](#3-printhub-giải-quyết-vấn-đề-như-thế-nào)
4. [Hệ thống làm được những gì](#4-hệ-thống-làm-được-những-gì)
5. [Vì sao cơ sở dữ liệu được thiết kế như vậy](#5-vì-sao-cơ-sở-dữ-liệu-được-thiết-kế-như-vậy)
6. [Nghiệp vụ lấy từ đâu — 5 nguồn](#6-nghiệp-vụ-lấy-từ-đâu--5-nguồn)
7. [Danh sách 114 Business Rules](#7-danh-sách-114-business-rules)
8. [Câu hỏi hay gặp khi bảo vệ](#8-câu-hỏi-hay-gặp-khi-bảo-vệ)

---

## 1. Giới thiệu dự án

**PrintHub** là một **sàn giao dịch trung gian (multi-vendor marketplace)** kết nối khách
hàng có nhu cầu in ấn / gia công với các cửa hàng in độc lập trong khu vực.

Điểm cốt lõi: khách hàng **cấu hình yêu cầu đúng một lần**, hệ thống **tự động tính giá
song song ở nhiều cửa hàng**, khách **so sánh và tự chọn** cửa hàng theo giá / thời gian /
khoảng cách / đánh giá, rồi theo dõi đơn hàng cho tới khi nhận hàng.

### Phạm vi dịch vụ — 3 nhóm, 3 cách tính giá khác nhau

Việc chia 3 nhóm không phải để cho phong phú, mà vì **mỗi nhóm có bản chất tính giá khác
nhau** — đây chính là lý do mẫu thiết kế **Strategy** trở thành bắt buộc chứ không phải
trang trí:

| Nhóm | Dịch vụ | Cách tính giá | Đơn vị |
|---|---|---|---|
| **A. Tài liệu** | In A4/A3 đen trắng & màu, photocopy, in bản vẽ A1, poster A2 | Theo **số trang** | trang |
| **B. Hoàn thiện** | Đóng gáy lò xo/nhiệt/bìa cứng, ép plastic, danh thiếp, decal | Theo **số lượng** (có chiết khấu theo bậc) | cái |
| **C. Gia công số** | In 3D FDM, cắt & khắc laser | Theo **khối lượng vật liệu** tiêu hao | gram |

Một cửa hàng chọn cung cấp nhóm nào tuỳ theo máy móc họ có: tiệm photo gần trường thường
chỉ có A + B, một xưởng makerspace chỉ có C, cửa hàng lớn có cả ba.

### 5 vai trò người dùng

```
Guest ──(đăng ký)──► Customer ──(nộp đơn + admin duyệt)──► Shop Owner ──(cấp quyền)──► Shop Staff
                                                                              
                                            Admin (quản trị sàn, tài khoản riêng)
```

| Vai trò | Làm được gì |
|---|---|
| **Guest** | Xem danh sách/chi tiết cửa hàng, xem đánh giá, đăng ký, đăng nhập |
| **Customer** | Tải tài liệu, so sánh báo giá, đặt hàng, thanh toán ví, theo dõi, đánh giá, khiếu nại |
| **Shop Owner** | Toàn quyền cửa hàng: bảng giá, máy móc, vật liệu, nhân viên, báo cáo doanh thu |
| **Shop Staff** | Chỉ vận hành đơn hàng của **đúng cửa hàng mình thuộc về** — không xem được bảng giá và doanh thu |
| **Admin** | Duyệt/treo cửa hàng, khoá tài khoản, danh mục dịch vụ, voucher, hoa hồng, phân xử khiếu nại |

> **Vì sao tách Shop Owner và Shop Staff?** Đây là quyết định có chủ đích để tạo ra
> **phân quyền theo phạm vi (scoped authorization)** — một chiều phân quyền phức tạp hơn
> phân quyền theo vai trò thông thường. Nhân viên không chỉ cần "là nhân viên", mà phải là
> "nhân viên **của đúng cửa hàng đó**". Đây là điểm kỹ thuật đáng nói nhất về mặt bảo mật
> trong dự án (xem [BR-74], [BR-82], [BR-83]).

---

## 2. Vấn đề thực tế (Problems)

Dự án xuất phát từ một tình huống rất cụ thể và quen thuộc: **sinh viên đi in tài liệu**.

### 2.1. Vấn đề 1 — Giá cả và thời gian hoàn toàn mù mờ

Kênh đặt hàng phổ biến hiện nay là **tài khoản chat cá nhân**: sinh viên gửi file qua Zalo
hoặc Messenger, mô tả yêu cầu bằng ngôn ngữ tự nhiên (*"in 2 mặt, giấy thường, đóng gáy lò
xo, 3 bản"*), rồi **chờ** cửa hàng trả lời.

Hệ quả:
- Khách **không biết giá trước khi in** — chỉ biết khi ra lấy hàng và trả tiền.
- Khách **không có cách nào so sánh** cửa hàng A với cửa hàng B, ngoài việc đi hỏi từng nơi.
- Khách **không biết bao giờ xong**, chỉ nhận được câu trả lời ước chừng "chiều nay" hoặc
  "mai nhé em".
- Không có mã đơn hàng nào để hai bên tham chiếu về sau khi có tranh chấp.

### 2.2. Vấn đề 2 — Kênh tiếp nhận đơn không có cấu trúc

Vấn đề này **đối xứng** — cửa hàng cũng chịu thiệt:
- Yêu cầu viết bằng văn xuôi → **dễ hiểu nhầm** (in 1 mặt hay 2 mặt? bìa màu gì?).
- File đến **lẫn lộn** giữa hàng chục cuộc hội thoại khác nhau → dễ in nhầm file.
- Cửa hàng **không thể phát tín hiệu về công suất hiện tại** của mình → nhận đơn không kịp
  làm, rồi phải thương lượng lại hoặc làm khách thất vọng.

### 2.3. Vấn đề 3 — Công suất không được chia sẻ giữa các cửa hàng

Vì mọi thứ diễn ra qua chat riêng, cửa hàng chỉ cạnh tranh bằng **vị trí địa lý**: thị
trường của một cửa hàng chỉ gồm những người đi ngang qua nó.

Kết quả rất lãng phí: vào mùa cao điểm (deadline đồ án), một cửa hàng quá tải phải từ chối
khách, trong khi **một cửa hàng cách đó 300 mét đang có máy rảnh** — nhưng không có cách nào
để công suất nhàn rỗi đó "hiện ra" trước mắt người khách đang xếp hàng.

### 2.4. Vấn đề 4 — Cửa hàng nhỏ không có kênh số nào khả thi

Các cửa hàng in nhỏ lẻ:
- Không đủ nguồn lực để tự làm website đặt hàng riêng.
- Không có hệ thống quản lý đơn hàng — ghi sổ tay hoặc nhớ trong đầu.
- Không có dữ liệu vận hành để biết dịch vụ nào lãi, máy nào chạy nhiều.

### Tóm tắt 4 vấn đề

| # | Vấn đề | Người chịu thiệt |
|---|---|---|
| 1 | Giá và thời gian mù mờ, không so sánh được | Khách hàng |
| 2 | Đặt hàng qua chat, không có cấu trúc, dễ sai sót | Cả hai bên |
| 3 | Công suất không chia sẻ được giữa các cửa hàng | Cả hai bên |
| 4 | Cửa hàng nhỏ không có kênh số hợp túi tiền | Cửa hàng |

---

## 3. PrintHub giải quyết vấn đề như thế nào

Mỗi giải pháp bên dưới **ánh xạ trực tiếp** tới một vấn đề ở Mục 2.

### 3.1. Giải quyết vấn đề 1 → Máy tính giá tự động, so sánh nhiều cửa hàng

Khách cấu hình yêu cầu một lần bằng **biểu mẫu có cấu trúc** (chọn dịch vụ, số trang, màu/
đen trắng, 1 mặt/2 mặt…). Hệ thống:

1. Lọc ra tập cửa hàng **đủ điều kiện** — đang hoạt động, có bán đúng dịch vụ đó, có máy
   không ở trạng thái Offline, còn vật liệu ([BR-31]).
2. Gọi **Quote Engine** (dịch vụ tính giá riêng, giao tiếp qua gRPC) cho **từng cửa hàng**,
   truyền vào bảng giá và các luật giá riêng của cửa hàng đó.
3. Nhận về giá + thời gian dự kiến, bổ sung khoảng cách và điểm đánh giá.
4. Trả về **danh sách xếp hạng, so sánh trực tiếp được**.

Khách nhìn thấy ngay: *cửa hàng A 160.000đ / 200 phút / ★4.6* cạnh *cửa hàng B 175.000đ /
180 phút / ★4.2* — và **tự quyết định**.

> **Lưu ý về mô hình:** PrintHub cố ý chọn mô hình **marketplace** (khách tự chọn cửa hàng),
> **không** phải mô hình auto-dispatch (hệ thống tự gán cửa hàng như app gọi xe). Lý do:
> quyết định chọn cửa hàng phụ thuộc vào ưu tiên cá nhân (rẻ nhất? gần nhất? nhanh nhất?)
> mà hệ thống không thể đoán thay khách.

### 3.2. Giải quyết vấn đề 2 → Đơn hàng có cấu trúc + máy trạng thái + lịch sử bất biến

- Mọi yêu cầu được nhập qua **trường dữ liệu có kiểu** (enum, số), không phải văn xuôi →
  không thể hiểu nhầm.
- Mỗi đơn có **mã đơn** dạng `PH-260727-0041` để hai bên tham chiếu.
- Đơn đi qua một **máy trạng thái** được định nghĩa chặt chẽ; mỗi lần chuyển trạng thái ghi
  lại **ai làm, lúc nào, lý do gì** vào bảng lịch sử **chỉ-ghi-thêm** ([BR-44], [BR-45]).

### 3.3. Giải quyết vấn đề 3 → Công suất được số hoá và hiển thị

Cửa hàng khai báo **máy móc** và **vật liệu** vào hệ thống. Trạng thái máy (Rảnh / Đang chạy
/ Bảo trì / Tắt) và tồn kho vật liệu **trực tiếp quyết định** cửa hàng đó có xuất hiện trong
kết quả báo giá hay không ([BR-31]).

Nghĩa là: cửa hàng đang rảnh **tự động** hiện ra trước mắt khách đang cần in — đúng thứ mà
mô hình chat không làm được.

### 3.4. Giải quyết vấn đề 4 → Cửa hàng nhỏ có sẵn công cụ quản lý, không tốn chi phí đầu tư

Chỉ cần đăng ký và được duyệt, cửa hàng có ngay: trang giới thiệu, bảng giá tự cấu hình,
quản lý máy móc/vật liệu/nhân viên, hàng đợi đơn hàng, và **báo cáo doanh thu**. Sàn chỉ thu
**hoa hồng theo đơn hoàn tất** (mặc định 10%, admin cấu hình được) — cửa hàng không phải trả
phí trước.

---

## 4. Hệ thống làm được những gì

### 4.1. Năm luồng nghiệp vụ chính

| # | Luồng | Mô tả ngắn | Có máy trạng thái? |
|---|---|---|---|
| 1 | **Đăng ký cửa hàng** | Nộp đơn → Admin duyệt → Hoạt động; có thể bị treo/phục hồi | ✅ |
| 2 | **So sánh báo giá & Đặt hàng** | Cấu hình → gọi Quote Engine → so sánh → chọn → trừ ví → đặt | — |
| 3 | **Sản xuất & Giao nhận** | Nhận đơn → sản xuất (bất đồng bộ) → sẵn sàng → hoàn tất | ✅ |
| 4 | **Nạp tiền & Thanh toán ví** | Yêu cầu nạp → VietQR → admin đối chiếu → cộng tiền | — |
| 5 | **Khiếu nại & Giải quyết** | Khách khiếu nại → shop phản hồi → chấp nhận **hoặc** leo thang lên admin | ✅ |

**Máy trạng thái đơn hàng** (luồng 3) — 12 trạng thái:

```
Draft → Quoted → AwaitingAcceptance ──┬─► Accepted ──► InProduction ──┬─► ReadyForPickup ──┐
                    │                 │                                │                    ├─► Completed
                    │                 │                                ├─► OutForDelivery ──┘
              Cancelled          Declined                              └─► ProductionFailed
                                (hoàn 100%)                                (thử lại hoặc từ chối + hoàn tiền)
```

### 4.2. 42 Use Case chia theo vai trò

| Nhóm | UC | Nội dung |
|---|---|---|
| Guest & chung | UC-01…10 | Đăng ký, đăng nhập, quên/đổi mật khẩu, hồ sơ, thông báo, tìm & xem cửa hàng |
| Customer | UC-11…24 | Yêu thích, tài liệu, so sánh báo giá, voucher, đặt/theo dõi/huỷ/đặt lại/xác nhận đơn, ví, đánh giá, khiếu nại |
| Shop Owner | UC-25…30 | Nộp đơn mở cửa hàng, hồ sơ, bảng giá & luật giá, máy móc/vật liệu, nhân viên, báo cáo doanh thu |
| Shop Staff | UC-31…35 | Hàng đợi, nhận/từ chối đơn, bắt đầu sản xuất, bàn giao, phản hồi khiếu nại |
| Admin | UC-36…42 | Duyệt/treo cửa hàng, quản lý tài khoản, danh mục dịch vụ & hoa hồng, voucher, phân xử khiếu nại, báo cáo sàn |

Danh sách endpoint đầy đủ: [`api-endpoints-full.md`](api-endpoints-full.md).

### 4.3. Bốn yêu cầu kỹ thuật bắt buộc và lý do nghiệp vụ

Điểm quan trọng khi bảo vệ: **không có kỹ thuật nào được đưa vào chỉ để "cho đủ"** — mỗi cái
đều có lý do nghiệp vụ đứng sau.

| Kỹ thuật | Dùng ở đâu | **Lý do nghiệp vụ** |
|---|---|---|
| **gRPC** | Quote Engine — dịch vụ tính giá độc lập (cổng 5090) | Một lần so sánh phải gọi tính giá **N lần** (N = số cửa hàng đủ điều kiện). Tách thành dịch vụ riêng để có thể scale/triển khai độc lập; hợp đồng kiểu tĩnh + Protobuf nhị phân phù hợp với gọi lặp nhiều lần. |
| **RabbitMQ** | Đẩy job sản xuất; `ProductionAgent` tiêu thụ | Một đơn in tài liệu chiếm máy **vài phút**, một đơn in 3D chiếm **hàng giờ** — **không thể** hoàn tất trong vòng đời một HTTP request. Việc dài hạn, có thể thất bại giữa chừng, phải sống sót qua restart → mô hình hàng đợi là biểu diễn đúng ([BR-89]). |
| **OData** | `/odata/Shops`, `/odata/Orders` (có `$expand`) | Màn hình tra cứu/quản trị cần lọc–sắp xếp–phân trang linh hoạt mà không bắt back-end viết endpoint riêng cho từng tổ hợp. Bộ lọc phạm vi **luôn được áp ở server trước** query của client ([BR-23]). |
| **Content negotiation** JSON/XML/CSV | Các endpoint báo cáo | **Nghị định 123/2020/NĐ-CP** quy định hoá đơn điện tử ở Việt Nam dùng định dạng **XML** → cùng một tài nguyên phải phục vụ JSON cho client của sàn và XML cho tích hợp kế toán. CSV thêm vào vì đa số cửa hàng nhỏ ghi sổ bằng Excel ([BR-114]). |

### 4.4. Các mẫu thiết kế (design pattern) đã dùng

| Pattern | Áp dụng ở đâu | Vì sao cần |
|---|---|---|
| **Strategy** | 3 chiến lược tính giá trong Quote Engine | 3 nhóm dịch vụ có công thức khác hẳn nhau ([BR-32]) |
| **Repository + Unit of Work** | Truy cập dữ liệu | Gom nhiều thao tác vào **một giao dịch** — đặt hàng phải trừ ví + tạo đơn + ghi sổ cái nguyên tử ([BR-42]) |
| **Specification** | Mọi truy vấn | Tái sử dụng điều kiện lọc, tránh lặp logic phân quyền |
| **Result\<T\>** | Toàn bộ tầng Application | Service trả kết quả kèm loại lỗi, controller chỉ ánh xạ sang HTTP status — không ném exception cho lỗi nghiệp vụ |
| **State Machine (table-driven)** | Vòng đời đơn hàng | Một bảng chuyển trạng thái duy nhất, tránh `if/else` rải rác khắp nơi |
| **Options** | Cấu hình JWT / RabbitMQ / SMTP / Cloudinary | Đổi hạ tầng không sửa code nghiệp vụ |

---

## 5. Vì sao cơ sở dữ liệu được thiết kế như vậy

Cơ sở dữ liệu gồm **23 bảng nghiệp vụ** (chưa kể bảng `__EFMigrationsHistory` do EF Core tự
quản lý). Dưới đây là **6 quyết định thiết kế** quan trọng nhất và lý do nghiệp vụ đứng sau —
đây chính là phần hay bị hỏi "tại sao?" khi bảo vệ.

### 5.1. Tách `Quotes` khỏi `Orders` — vì "xem giá" và "mua thật" là hai việc khác nhau

| | `Quotes` (báo giá) | `Orders` (đơn hàng) |
|---|---|---|
| Sinh ra khi nào | Mỗi lần khách **so sánh giá** | Chỉ khi khách **thực sự đặt** |
| Số lượng | **Nhiều** — 1 lần so sánh sinh ra N dòng (N = số cửa hàng) | **Ít** — chỉ 1 dòng nếu khách chốt |
| Có mất tiền không | ❌ Không | ✅ Có |
| Hết hạn | ✅ 24 giờ ([BR-34]) | Không áp dụng |

**Lý do:** khách rất hay so giá nhiều lần rồi **không đặt gì cả**. Nếu gộp chung, bảng
`Orders` — bảng chứng từ tài chính, phải sạch — sẽ đầy rác. Ngoài ra báo giá **phải hết hạn**
vì giá giấy và vật liệu thay đổi theo thời gian.

### 5.2. Snapshot (chụp ảnh) dữ liệu tại thời điểm chốt — bảo vệ điều khoản đã thoả thuận

Đây là nguyên tắc lặp lại ở **4 chỗ** trong CSDL:

| Cột | Chụp lại cái gì | Nếu không có thì sao |
|---|---|---|
| `Orders.QuoteId` + `SubTotal` | Giá tại thời điểm đặt | Cửa hàng đổi bảng giá → đơn cũ đổi giá theo → sai thoả thuận ([BR-41]) |
| `OrderItems.UnitPrice`, `LineTotal` | Giá từng dòng đã áp luật | Như trên, ở mức chi tiết |
| `Orders.CommissionRate` | Tỉ lệ hoa hồng lúc hoàn tất | Admin đổi hoa hồng → báo cáo doanh thu quá khứ bị sai lệch ([BR-52]) |
| `OrderItems.SnapshotFileName` | Tên file lúc đặt | Khách đổi tên/xoá file → đơn cũ hiển thị sai tên tài liệu |

**Nguyên tắc chung:** chứng từ đã chốt thì **bất biến**. Đây cũng là yêu cầu pháp lý (xem
Mục 6.1 — Luật Giao dịch điện tử về tính toàn vẹn của dữ liệu giao dịch).

### 5.3. Ba bảng quan hệ nhiều-nhiều **có thuộc tính**

Không phải bảng nối thuần (chỉ 2 khoá ngoại) — mỗi bảng mang dữ liệu riêng:

| Bảng | Nối gì với gì | Thuộc tính riêng | Vai trò nghiệp vụ |
|---|---|---|---|
| **`ShopServices`** | Shop ⇄ ServiceType | Đơn giá, phí cài đặt, số lượng tối thiểu/tối đa, thời gian sản xuất | **Chính là bảng giá** — trái tim của Quote Engine |
| **`ShopStaff`** | User ⇄ Shop | Chức danh, ngày vào, còn hiệu lực, ai mời | Nguồn dữ liệu cho **phân quyền theo cửa hàng** |
| **`Favourites`** | User ⇄ Shop | Ghi chú riêng, ngày thêm | Cửa hàng yêu thích của khách |

### 5.4. Ba bảng **chỉ-ghi-thêm** (append-only) — dùng làm bằng chứng

| Bảng | Ghi lại gì | Vì sao không cho sửa/xoá |
|---|---|---|
| `OrderStatusHistories` | Từng bước chuyển trạng thái đơn (ai, lúc nào, lý do) | Dùng làm **bằng chứng khi xử lý khiếu nại** — vd khách kiện "giao trễ", admin cần biết chính xác đơn vào sản xuất lúc mấy giờ ([BR-45]) |
| `WalletTransactions` | Mọi biến động tiền, **kèm số dư sau giao dịch** | Sổ cái **tự kiểm chứng được**: cộng dồn phải khớp số dư hiện tại ([BR-58]). Không cho sửa số dư trực tiếp — sai thì ghi giao dịch điều chỉnh ([BR-104]) |
| `AuditLogs` | Hành động nhạy cảm của admin (trạng thái trước/sau dạng JSON) | Truy trách nhiệm ([BR-97], [BR-100]) |

### 5.5. Xoá mềm (soft-delete) thay vì xoá cứng

Các bảng `Users`, `Shops`, `Machines`, `Materials`, `ServiceTypes`, `DocumentFiles` đều có cột
`IsDeleted`. EF Core tự động lọc bỏ dòng đã xoá ở mọi truy vấn.

**Lý do:** những bảng này đều bị đơn hàng cũ tham chiếu tới. Xoá cứng một loại dịch vụ sẽ làm
**mất luôn ý nghĩa của các đơn hàng lịch sử** đang dùng nó ([BR-107]).

### 5.6. Hành vi xoá (`CASCADE` vs `NO ACTION`) được chọn có chủ đích

Đây là chi tiết dễ bị hỏi nhất. Cùng là khoá ngoại trỏ về `Shops`, nhưng hành vi khác nhau:

| Bảng con | Hành vi | Lý do |
|---|---|---|
| `Machines`, `Materials`, `ShopServices`, `ShopStaff` | **CASCADE** | Là **tài sản/thành phần** của cửa hàng — mất cửa hàng thì dữ liệu này vô nghĩa |
| `Orders`, `Quotes`, `Reviews`, `Complaints`, `Favourites` | **NO ACTION** | Là **chứng từ / dữ liệu phía khách** — phải giữ nguyên vẹn, không được xoá dây chuyền |

Tương tự với `Users`: `RefreshTokens`/`Notifications` dùng CASCADE (dữ liệu phái sinh, bỏ
được), còn `AuditLogs`/`WalletTransactions` dùng NO ACTION (bằng chứng, phải giữ).

**Lợi ích kỹ thuật kèm theo:** cách chọn này cũng tránh được lỗi *cascade cycle* mà SQL Server
cấm — khi nhiều đường dẫn xoá dây chuyền cùng chỉ về một bảng.

---

## 6. Nghiệp vụ lấy từ đâu — 5 nguồn

> **Nghiệp vụ (business logic / business rules)** là tập hợp các **quy tắc ràng buộc** mà hệ
> thống phải tuân theo để phản ánh đúng cách công việc diễn ra ngoài đời thực — ví dụ *"chỉ
> đơn đã hoàn tất mới được đánh giá"*, *"từ chối đơn thì phải hoàn 100% tiền"*. Nó **không
> phải** yêu cầu kỹ thuật, mà là **luật chơi của lĩnh vực kinh doanh**.

Nghiệp vụ của PrintHub được rút ra từ **5 nguồn** dưới đây, xếp theo mức độ ràng buộc.

### 6.1. Nguồn 1 — Pháp luật Việt Nam (ràng buộc bắt buộc)

Đây là nguồn **mạnh nhất**: vi phạm là sai luật, không phải chỉ là thiết kế kém.

| Văn bản | Nội dung liên quan | Sinh ra quy tắc nào trong PrintHub |
|---|---|---|
| **Nghị định 52/2013/NĐ-CP** (sửa đổi bởi **85/2021/NĐ-CP**) về thương mại điện tử | Sàn TMĐT phải: **xác minh danh tính người bán**, có **cơ chế xử lý khiếu nại**, **công khai giá minh bạch** trước khi giao dịch | → Bắt buộc **quy trình duyệt cửa hàng** ([BR-68]) <br> → Bắt buộc **luồng khiếu nại có leo thang lên sàn** ([BR-62]…[BR-65]) <br> → Bắt buộc **hiển thị và lưu bảng chi tiết giá** trước khi khách xác nhận ([BR-33], [BR-41]) |
| **Nghị định 13/2023/NĐ-CP** về bảo vệ dữ liệu cá nhân | Phải có **sự đồng ý rõ ràng**, có **cơ chế xoá dữ liệu**, **giới hạn mục đích xử lý** | → Cam kết bản quyền khi tải file ([BR-28]) <br> → Hạn lưu trữ tài liệu (`PurgeAfter`) <br> → Không lộ đường dẫn file, phục vụ qua endpoint kiểm tra quyền ([BR-30]) |
| **Nghị định 123/2020/NĐ-CP** + **Thông tư 78/2021/TT-BTC** về hoá đơn điện tử | Hoá đơn điện tử dùng định dạng **XML** theo lược đồ quy định | → **Content negotiation** JSON/XML/CSV là tính năng nghiệp vụ, không phải trình diễn kỹ thuật ([BR-114]) |
| **Luật Giao dịch điện tử 2023** (Luật 20/2023/QH15) | Yêu cầu **tính toàn vẹn và khả năng truy xuất** của dữ liệu giao dịch điện tử dùng làm bằng chứng | → **Lịch sử trạng thái bất biến** ([BR-44], [BR-45]) <br> → **Snapshot báo giá** vào đơn ([BR-41]) |
| **Luật Sở hữu trí tuệ 2005** (sửa đổi 2022) | Bảo hộ tác phẩm chống sao chép trái phép | → In ấn **về bản chất là hành vi sao chép** → bắt buộc khách **cam kết có quyền in** ([BR-28]) <br> → Cửa hàng được từ chối đơn với lý do `CopyrightConcern` ([BR-85]) |

> ⚠️ **Lưu ý khi bảo vệ:** các văn bản trên là có thật, nhưng **hãy tự mở đọc điều khoản cụ
> thể trước khi nộp**. Nếu giảng viên hỏi *"điều nào của nghị định nói vậy?"* mà chưa đọc thì
> mất điểm nặng hơn là không trích dẫn. Tuyệt đối không trích số điều/khoản chưa kiểm chứng.

### 6.2. Nguồn 2 — Quan sát thực tế quy trình tiệm in

Nghiệp vụ vận hành lấy từ **cách các tiệm in thực sự làm việc**, không phải tự nghĩ ra:

| Quan sát thực tế | Quy tắc tương ứng |
|---|---|
| Tiệm in **có quyền từ chối** đơn (hết giấy, máy hỏng, file lỗi, không làm được) | [BR-85], [BR-86] — từ chối **bắt buộc nêu lý do** và **hoàn 100% tiền** |
| Nhận đơn rồi thì tiệm đã **giữ chỗ công suất** cho khách | [BR-47] — huỷ khi chưa nhận: hoàn đủ; huỷ sau khi đã nhận: **có phí huỷ** cho tiệm |
| Đang in dở thì **không thể huỷ** — giấy mực đã tốn | [BR-48] — cấm huỷ khi đã vào sản xuất |
| Tiệm có **nhiều máy**, phân công đơn cho máy cụ thể | `Orders.MachineId` — do **tiệm gán**, khách không chọn máy |
| Máy đang chạy đơn thì **không thể tắt** | [BR-75] |
| Nhân viên **không được biết giá vốn và doanh thu** của chủ | [BR-74], [BR-82], [BR-24] |
| Khách trả tiền trước, nhận hàng sau (đặc thù in ấn — sản phẩm cá nhân hoá, không bán lại được) | Mô hình **ví trả trước**, trừ tiền ngay khi đặt ([BR-40]) |

### 6.3. Nguồn 3 — Chuẩn mực ngành từ sàn TMĐT và marketplace quốc tế

**Xometry** (https://www.xometry.com) và **Hubs** (https://www.hubs.com) là hai sàn gia công
theo yêu cầu quốc tế. Bài học rút ra:

| Học được gì | Áp dụng vào PrintHub |
|---|---|
| **Instant quoting** — báo giá tự động từ thông số, không cần người | Quote Engine tính giá tự động, không chờ tiệm trả lời |
| Báo giá **có thời hạn**, không vô thời hạn | [BR-34] — quote hết hạn sau 24 giờ |
| Nhà cung cấp phải được **thẩm định trước** khi lên sàn | [BR-68] — cửa hàng phải qua duyệt |
| Bảng phân tích giá chi tiết cho khách xem | [BR-33] — lưu `BreakdownJson` từng luật giá đã áp |

**Khác biệt của PrintHub:** Xometry/Hubs phục vụ **khách công nghiệp B2B thị trường phương
Tây** và **không có in tài liệu**. PrintHub áp dụng đúng cơ chế đó cho nhu cầu **in ấn hằng
ngày, quy mô cá nhân, tại địa phương** — chính là khoảng trống mà kênh chat đang phục vụ rất
tệ.

Từ các sàn TMĐT nội địa (Shopee/Lazada), lấy các chuẩn mực đã quen thuộc với người Việt: đánh
giá sau khi nhận hàng, voucher giảm giá, ví điện tử, theo dõi đơn hàng theo timeline.

### 6.4. Nguồn 4 — Ràng buộc bảo mật và toàn vẹn dữ liệu (chuẩn kỹ thuật chung)

| Chuẩn mực | Quy tắc |
|---|---|
| Không bao giờ lưu mật khẩu dạng thô | [BR-3] — BCrypt kèm salt riêng |
| Thông báo lỗi đăng nhập không được tiết lộ email có tồn tại hay không | [BR-5], [BR-8] — chống dò tài khoản (user enumeration) |
| Danh tính **luôn lấy từ token**, không bao giờ từ tham số client gửi lên | [BR-13] — chống giả mạo id |
| Phân quyền phải áp ở **server**, trước khi client lọc | [BR-23], [BR-53], [BR-83] |
| Đăng xuất / đổi mật khẩu / khoá tài khoản phải **thu hồi được phiên** | [BR-9], [BR-10], [BR-103] |

### 6.5. Nguồn 5 — Quyết định phạm vi của chính dự án (cắt giảm có chủ đích)

Một số nghiệp vụ được **cố ý đơn giản hoá** để phù hợp quy mô đồ án. Nêu rõ ra khi bảo vệ sẽ
được đánh giá cao hơn là để giảng viên tự phát hiện:

| Đã đơn giản hoá | Thực tế đầy đủ sẽ là | Vì sao chấp nhận được |
|---|---|---|
| Ví chỉ có **số dư + sổ cái** | Có tạm giữ (reserve) và quyết toán (capture) như cổng thanh toán thật | Không ảnh hưởng logic đặt hàng; giữ được tính kiểm chứng của sổ cái |
| Vật liệu chỉ **cảnh báo sắp hết**, không trừ kho theo đơn | Đặt chỗ vật liệu, trừ kho chính xác từng đơn | Tiệm in thực tế cũng quản kho khá thô; tránh phức tạp không cần thiết |
| **Không có luồng chi trả** cho cửa hàng | Đối soát và chuyển tiền định kỳ cho tiệm | Chỉ **báo cáo hoa hồng**; chi trả là bài toán tài chính riêng |
| Sản xuất được **giả lập** bởi agent | Tích hợp máy in thật qua driver | Không có phần cứng; agent mô phỏng đúng đặc tính *dài hạn, bất đồng bộ, có thể lỗi* |
| Nạp tiền **admin xác nhận thủ công** | Webhook tự động từ ngân hàng | Không có tài khoản doanh nghiệp thật; luồng nghiệp vụ vẫn đúng |

---

## 7. Danh sách 114 Business Rules

Nguồn gốc: [`2_SRS_Part3_Requirements.md`](2_SRS_Part3_Requirements.md) §5.1.

### 7.1. Xác thực & Tài khoản (BR-1 → BR-20)

| ID | Quy tắc |
|---|---|
| BR-1 | Mỗi địa chỉ email chỉ gắn với **đúng một** tài khoản. |
| BR-2 | Tài khoản mới đăng ký mặc định là vai trò **Customer**, số dư ví bằng 0. |
| BR-3 | Mật khẩu phải lưu dưới dạng **băm BCrypt kèm salt riêng**; tuyệt đối không lưu dạng thô. |
| BR-4 | Mật khẩu tối thiểu 8 ký tự, có ít nhất 1 chữ hoa, 1 chữ thường và 1 chữ số. |
| BR-5 | Đăng nhập thất bại phải trả **thông báo chung chung**, không tiết lộ email có tồn tại hay không. |
| BR-6 | Tài khoản trạng thái **Locked** không được cấp access token. |
| BR-7 | Ứng dụng desktop chỉ chấp nhận vai trò Shop Owner, Shop Staff và Admin; tài khoản Customer bị từ chối. |
| BR-8 | Yêu cầu đặt lại mật khẩu phải trả **cùng một thông báo trung tính** dù email có đăng ký hay không. |
| BR-9 | Đặt lại hoặc đổi mật khẩu thành công phải **thu hồi toàn bộ refresh token** của tài khoản. |
| BR-10 | Đăng xuất phải thu hồi refresh token **phía server**; chỉ xoá ở client là không đủ. |
| BR-11 | Mọi sự kiện đăng xuất và đổi mật khẩu phải được ghi vào nhật ký kiểm toán. |
| BR-12 | Mật khẩu mới **không được trùng** mật khẩu hiện tại. |
| BR-13 | Người dùng chỉ được xem và thao tác trên bản ghi của chính mình; **danh tính lấy từ access token**, không bao giờ từ tham số client gửi lên. |
| BR-14 | Không endpoint nào được trả về mã băm mật khẩu, kể cả dạng che dấu. |
| BR-15 | Tên hiển thị phải từ 2 đến 100 ký tự. |
| BR-16 | Chủ tài khoản **không được tự đổi** email và vai trò; cả hai cần admin thực hiện. |
| BR-17 | Mọi thay đổi hồ sơ phải ghi nhật ký kiểm toán kèm id người dùng và thời điểm. |
| BR-18 | Thông báo phải sắp xếp **mới nhất trước**. |
| BR-19 | Thông báo được đánh dấu đã đọc khi đã hiển thị cho người nhận. |
| BR-20 | Người dùng chỉ xem được thông báo gửi tới chính tài khoản mình. |

### 7.2. Tìm kiếm, Yêu thích & Tài liệu (BR-21 → BR-30)

| ID | Quy tắc |
|---|---|
| BR-21 | Chỉ cửa hàng trạng thái **Active** mới xuất hiện trong tìm kiếm và đủ điều kiện báo giá. |
| BR-22 | Mọi endpoint danh sách phải trả **cấu trúc phân trang chuẩn** gồm số trang, cỡ trang, tổng số bản ghi, tổng số trang. |
| BR-23 | Bộ lọc phạm vi phía server phải được áp **trước khi** đánh giá bất kỳ truy vấn OData nào từ client. |
| BR-24 | Chỉ hiển thị công khai các dòng bảng giá đang bật; **giá vốn vật liệu của cửa hàng không bao giờ lộ ra ngoài**. |
| BR-25 | Một cửa hàng chỉ xuất hiện tối đa 1 lần trong danh sách yêu thích của khách (ràng buộc unique theo cặp). |
| BR-26 | Khách chỉ xem được danh sách yêu thích của chính mình. |
| BR-27 | File tải lên phải đúng danh sách định dạng cho phép và không vượt giới hạn dung lượng. |
| BR-28 | Không thể tải file lên nếu khách **chưa chấp nhận cam kết về quyền sở hữu trí tuệ**. |
| BR-29 | File đang gắn với đơn **chưa hoàn tất** thì không được xoá. |
| BR-30 | **Đường dẫn lưu trữ file không bao giờ lộ ra client**; file chỉ phục vụ qua endpoint có kiểm tra quyền sở hữu hoặc quan hệ thực hiện đơn đang hoạt động. |

### 7.3. Báo giá & Voucher (BR-31 → BR-38)

| ID | Quy tắc |
|---|---|
| BR-31 | Cửa hàng chỉ đủ điều kiện báo giá khi: đang **Active**, cung cấp **mọi loại dịch vụ** trong đơn, có **máy không Offline** cho các dịch vụ đó, và **còn vật liệu** cần thiết. |
| BR-32 | Chiến lược tính giá được quyết định bởi mô hình giá của loại dịch vụ: theo trang / theo đơn vị có bậc số lượng / theo vật liệu và thời gian máy. |
| BR-33 | Mọi báo giá phải lưu **bảng phân tích chi tiết** cho biết những luật giá nào đã được áp và ảnh hưởng ra sao. |
| BR-34 | Mọi báo giá đều có **thời điểm hết hạn**; báo giá hết hạn không dùng để đặt đơn được. |
| BR-35 | Khi Quote Engine không khả dụng, hệ thống trả **giá tham khảo được đánh dấu là chưa chính thức** thay vì để request thất bại. |
| BR-36 | Voucher chỉ áp dụng được trong khoảng thời gian hiệu lực và khi đang bật. |
| BR-37 | Voucher chỉ áp dụng khi tổng đơn đạt **mức tối thiểu**; giảm giá theo phần trăm bị chặn trên bởi **mức giảm tối đa**. |
| BR-38 | Số lượt đã dùng của voucher chỉ tăng khi **đặt đơn thành công**, không tăng lúc áp mã ở màn hình thanh toán. |

### 7.4. Đặt hàng & Vòng đời đơn (BR-39 → BR-53)

| ID | Quy tắc |
|---|---|
| BR-39 | Chỉ đặt được đơn từ một báo giá **hợp lệ và chưa hết hạn**. |
| BR-40 | Chỉ đặt được đơn khi **số dư ví ≥ tổng tiền đơn**. |
| BR-41 | Bảng phân tích giá của báo giá được **chụp lại (snapshot) vào đơn** khi đặt; thay đổi bảng giá sau này **không bao giờ** làm thay đổi điều khoản đã thoả thuận. |
| BR-42 | Đặt đơn, trừ ví, tăng lượt voucher và ghi lịch sử phải diễn ra trong **một giao dịch duy nhất**. |
| BR-43 | Khung giờ hẹn lấy hàng phải nằm trong giờ mở cửa của cửa hàng. |
| BR-44 | Mọi lần chuyển trạng thái đơn phải được ghi vào lịch sử **chỉ-ghi-thêm** kèm người thực hiện, vai trò, thời điểm và lý do. |
| BR-45 | Bản ghi lịch sử trạng thái đơn **không bao giờ được sửa hoặc xoá**. |
| BR-46 | Khách chỉ được huỷ đơn khi trạng thái là **AwaitingAcceptance** hoặc **Accepted**. |
| BR-47 | Huỷ ở AwaitingAcceptance được **hoàn đủ**; huỷ ở Accepted bị **trừ phí huỷ**, phần phí này thuộc về cửa hàng. |
| BR-48 | Đơn đã vào sản xuất thì khách **không được huỷ**. |
| BR-49 | Chỉ đơn **Completed** mới dùng làm nguồn để đặt lại. |
| BR-50 | Đặt lại chỉ sao chép **cấu hình**, không bao giờ sao chép giá; đơn mới phải được báo giá theo mức giá hiện hành. |
| BR-51 | Đơn đạt trạng thái **Completed** khi khách xác nhận đã nhận **hoặc** nhân viên cửa hàng ghi nhận bàn giao. |
| BR-52 | Hoa hồng sàn được tính và ghi nhận **tại thời điểm hoàn tất đơn**, dùng tỉ lệ hoa hồng hiện hành lúc đó. |
| BR-53 | Truy vấn đơn hàng của khách bị **giới hạn ở phía server** trong phạm vi đơn của chính họ, bất kể client gửi bộ lọc gì. |

### 7.5. Ví điện tử (BR-54 → BR-58)

| ID | Quy tắc |
|---|---|
| BR-54 | Số tiền nạp phải nằm trong khoảng tối thiểu – tối đa đã cấu hình. |
| BR-55 | Nạp ví yêu cầu tài khoản đã **xác thực email**. |
| BR-56 | Mỗi yêu cầu nạp đang chờ mang một **mã tham chiếu duy nhất**, được ràng buộc unique ở cấp cơ sở dữ liệu. |
| BR-57 | Yêu cầu nạp không được xác nhận trong thời hạn hiệu lực sẽ chuyển sang **Expired** và **không ảnh hưởng số dư**. |
| BR-58 | Mọi giao dịch ví phải ghi lại **số dư sau giao dịch**, để sổ cái có thể **đối chiếu độc lập** với số dư đang lưu. |

### 7.6. Đánh giá & Khiếu nại (BR-59 → BR-65)

| ID | Quy tắc |
|---|---|
| BR-59 | Chỉ khách sở hữu một đơn **Completed** mới được đánh giá cửa hàng đã thực hiện đơn đó. |
| BR-60 | Mỗi đơn có **tối đa 1 đánh giá**, ràng buộc bằng unique constraint. |
| BR-61 | Điểm trung bình và số lượt đánh giá của cửa hàng phải được tính lại **trong cùng giao dịch** tạo đánh giá. |
| BR-62 | Khiếu nại chỉ được tạo với đơn **Completed** và **trong thời hạn khiếu nại** đã cấu hình. |
| BR-63 | Tại mỗi thời điểm, mỗi đơn có **tối đa 1 khiếu nại đang mở**. |
| BR-64 | Cách giải quyết khiếu nại là **in lại miễn phí** hoặc **hoàn tiền vào ví**; số tiền hoàn **không vượt quá** tổng giá trị đơn. |
| BR-65 | Khiếu nại **tự động leo thang** khi cửa hàng không phản hồi trong thời hạn quy định. |

### 7.7. Đăng ký & Quản lý cửa hàng (BR-66 → BR-84)

| ID | Quy tắc |
|---|---|
| BR-66 | Mỗi người dùng tại một thời điểm chỉ được có **tối đa 1 cửa hàng** ở trạng thái PendingReview hoặc Active. |
| BR-67 | Đơn xin mở cửa hàng phải điền đủ mọi trường bắt buộc trước khi nộp duyệt. |
| BR-68 | Cửa hàng **không được nhận đơn** cho tới khi admin phê duyệt. |
| BR-69 | **Giờ đóng cửa phải sau giờ mở cửa.** |
| BR-70 | Chủ cửa hàng **không tự đổi được trạng thái** cửa hàng; treo và phục hồi là hành động của admin. |
| BR-71 | Đơn giá và phí cài đặt **không được âm**; hệ số nhân giá phải **dương**. |
| BR-72 | Các bậc số lượng trong cùng một dòng bảng giá **không được chồng lấn nhau**. |
| BR-73 | Tắt một dòng bảng giá sẽ gỡ dịch vụ khỏi hiển thị công khai và khỏi diện báo giá, nhưng **không ảnh hưởng đơn đã có**. |
| BR-74 | **Chỉ Shop Owner** được xem và sửa bảng giá; **Shop Staff không có quyền truy cập**. |
| BR-75 | Máy đang được gán cho một đơn đang sản xuất thì **không thể chuyển sang Offline**. |
| BR-76 | Số lượng tồn kho vật liệu **không được âm**. |
| BR-77 | Cảnh báo sắp hết hàng được bật khi tồn kho **chạm hoặc xuống dưới** ngưỡng đã cấu hình. |
| BR-78 | Chỉ cấp quyền nhân viên cho **tài khoản đã tồn tại và đang Active** trên nền tảng. |
| BR-79 | Chủ cửa hàng **không thể tự thêm chính mình** làm nhân viên cửa hàng của mình. |
| BR-80 | Thu hồi quyền nhân viên **cắt quyền thao tác ngay lập tức** nhưng vẫn **giữ nguyên danh tính người thực hiện** đã ghi trong các hành động lịch sử. |
| BR-81 | Báo cáo doanh thu **chỉ tính các đơn Completed**. |
| BR-82 | **Chỉ Shop Owner** được xem báo cáo doanh thu; **Shop Staff không được xem số liệu doanh thu**. |
| BR-83 | Nhân sự cửa hàng chỉ xem được đơn thuộc cửa hàng mình là thành viên; **phạm vi được ép ở phía server**. |
| BR-84 | Nhân viên thuộc nhiều cửa hàng thì mỗi phiên làm việc chỉ thao tác trong **một ngữ cảnh cửa hàng** tại một thời điểm. |

### 7.8. Vận hành đơn hàng phía cửa hàng (BR-85 → BR-95)

| ID | Quy tắc |
|---|---|
| BR-85 | Từ chối đơn **bắt buộc chọn lý do** từ tập lý do đã định nghĩa. |
| BR-86 | Từ chối đơn kích hoạt **hoàn tiền 100% vô điều kiện, tự động** về ví khách. |
| BR-87 | Đơn chỉ được nhận hoặc từ chối khi trạng thái là **AwaitingAcceptance**. |
| BR-88 | Chỉ bắt đầu sản xuất được với đơn ở trạng thái **Accepted**, và chỉ với **máy đang Rảnh** thuộc đúng loại yêu cầu. |
| BR-89 | Công việc sản xuất **phải được điều phối qua message broker** và **không bao giờ thực thi bên trong một HTTP request**. |
| BR-90 | Trình xử lý sự kiện sản xuất phải **idempotent**; sự kiện gửi lại cho đơn đã ở trạng thái đích sẽ bị bỏ qua. |
| BR-91 | Chạy lại sản xuất sau khi lỗi **không tính thêm phí** cho khách. |
| BR-92 | Đơn chỉ được bàn giao khi trạng thái là **ReadyForPickup** hoặc **OutForDelivery**. |
| BR-93 | Cửa hàng chỉ phản hồi khiếu nại khi khiếu nại đang ở trạng thái **Open**. |
| BR-94 | Hành động **từ chối, treo, khoá, phân xử** đều **bắt buộc có lý do được ghi lại**. |
| BR-95 | Cửa hàng **không được giải quyết khiếu nại của cửa hàng khác**; phạm vi ép ở phía server. |

### 7.9. Quản trị sàn (BR-96 → BR-112)

| ID | Quy tắc |
|---|---|
| BR-96 | Duyệt đơn mở cửa hàng sẽ **nâng vai trò người nộp lên Shop Owner** đồng thời **vẫn giữ khả năng đặt hàng như khách**. |
| BR-97 | Quyết định duyệt/từ chối cửa hàng phải ghi **admin nào quyết định và thời điểm** vào nhật ký kiểm toán. |
| BR-98 | Đơn mở cửa hàng chỉ được quyết định khi đang ở trạng thái **PendingReview**. |
| BR-99 | Cửa hàng bị treo bị loại khỏi tìm kiếm và diện báo giá, **nhưng các đơn đang thực hiện vẫn được hoàn tất**. |
| BR-100 | Treo và phục hồi cửa hàng phải được ghi vào nhật ký kiểm toán. |
| BR-101 | Treo cửa hàng là **hành động đảo ngược được**; hệ thống **không hỗ trợ xoá vĩnh viễn** cửa hàng. |
| BR-102 | Admin **không thể tự khoá tài khoản của chính mình**. |
| BR-103 | Khoá tài khoản sẽ **thu hồi toàn bộ refresh token** của tài khoản đó. |
| BR-104 | **Số dư ví không bao giờ được sửa trực tiếp**; điều chỉnh phải ghi thành giao dịch loại Adjustment kèm lý do bắt buộc. |
| BR-105 | Mã loại dịch vụ phải **duy nhất** trên toàn danh mục nền tảng. |
| BR-106 | **Không được đổi mô hình giá** của một loại dịch vụ khi đã tồn tại đơn hàng dùng loại dịch vụ đó. |
| BR-107 | Loại dịch vụ đang được đơn hàng tham chiếu thì **tắt đi chứ không xoá**, để bảo toàn toàn vẹn tham chiếu. |
| BR-108 | Mã voucher phải duy nhất; giảm giá phần trăm không vượt quá 100 và giá trị giảm phải dương. |
| BR-109 | Ngày kết thúc hiệu lực của voucher phải sau ngày bắt đầu. |
| BR-110 | Chỉ khiếu nại ở trạng thái **Escalated** mới được admin phân xử. |
| BR-111 | Phán quyết của admin với khiếu nại đã leo thang là **cuối cùng** trong hệ thống. |
| BR-112 | Khiếu nại được chấp thuận thì tiền hoàn được **cộng vào ví khách** và **ghi nhận đối với cửa hàng**. |

### 7.10. Báo cáo (BR-113 → BR-114)

| ID | Quy tắc |
|---|---|
| BR-113 | Báo cáo là **tổng hợp chỉ-đọc** tính từ các đơn Completed và **không bao giờ thay đổi dữ liệu**. |
| BR-114 | Tài nguyên báo cáo phải **tuần tự hoá được thành JSON, XML và CSV**, lựa chọn bằng header HTTP `Accept`. |

---

## 8. Câu hỏi hay gặp khi bảo vệ

| Câu hỏi | Trả lời ngắn gọn |
|---|---|
| **Vì sao chọn marketplace mà không tự động gán cửa hàng?** | Quyết định chọn cửa hàng phụ thuộc ưu tiên cá nhân (rẻ / gần / nhanh / đánh giá tốt) mà hệ thống không đoán thay được. Đây là quyết định phạm vi có chủ đích, ghi rõ từ đầu. |
| **Vì sao cần gRPC, dùng REST không được à?** | Được — REST vẫn chạy. gRPC được chọn vì một lần so sánh gọi tính giá **N lần** (N = số cửa hàng), Protobuf nhị phân + hợp đồng kiểu tĩnh phù hợp hơn với gọi lặp; đồng thời mô phỏng đúng kiến trúc microservice (Quote Engine scale độc lập được). Cách triển khai là **thật**: 2 tiến trình riêng, tắt Quote Engine thì API vẫn sống và chuyển sang giá tham khảo. |
| **Vì sao sản xuất phải qua RabbitMQ?** | [BR-89]. Đơn in 3D chiếm máy **hàng giờ** — không thể giữ một HTTP request mở suốt thời gian đó. Việc dài hạn, có thể lỗi giữa chừng, phải sống sót qua restart → hàng đợi tin nhắn là biểu diễn kiến trúc đúng. |
| **Đây có phải Domain-Driven Design không?** | **Không phải DDD đầy đủ.** Đây là **Clean Architecture** — phân lớp với hướng phụ thuộc chỉ đi vào trong. Logic nghiệp vụ đặt ở tầng Service (mẫu Transaction Script) thay vì đặt trong Entity (Rich Domain Model của DDD thật). Đây là lựa chọn có chủ đích để giữ đơn giản và dễ kiểm thử ở quy mô đồ án. |
| **Vì sao tách project riêng mà không chỉ chia thư mục?** | Thư mục chỉ là **quy ước** — vi phạm vẫn biên dịch được. Project riêng biến nó thành **luật cứng do trình biên dịch ép**: `Domain.csproj` không tham chiếu project nào, nên code trong Domain **không thể** gọi sang Infrastructure dù vô tình hay cố ý. |
| **Đăng xuất rồi mà JWT cũ vẫn dùng được?** | Đúng, và đây là **giới hạn cố hữu của JWT**, không phải lỗi. Đăng xuất thu hồi **refresh token** (chặn xin token mới); access token đã cấp vẫn hợp lệ tới khi hết hạn. Giảm thiểu bằng cách đặt thời gian sống rất ngắn — **15 phút** chính là "mức thiệt hại tối đa chấp nhận được". |
| **Số liệu thị trường lấy ở đâu?** | Tài liệu **cố ý không bịa số**. Các mục cần số liệu được đánh dấu `[CẦN BỔ SUNG]` kèm gợi ý nguồn chính thống (Sách trắng TMĐT – Bộ Công Thương; Cục Xuất bản, In và Phát hành; Ngân hàng Nhà nước). Lập luận định tính chặt chẽ tốt hơn một con số sai. |

---

## Tài liệu liên quan

| Tài liệu | Nội dung |
|---|---|
| [`1_Project_Introduction.md`](1_Project_Introduction.md) | Báo cáo giới thiệu dự án đầy đủ (bối cảnh, pháp lý, đối thủ, thị trường) |
| [`2_SRS_Part1_Overview.md`](2_SRS_Part1_Overview.md) | Sơ đồ ngữ cảnh, 5 luồng chính, danh sách 42 UC, ERD |
| [`2_SRS_Part2_UseCases.md`](2_SRS_Part2_UseCases.md) | Đặc tả chi tiết 42 use case |
| [`2_SRS_Part3_Requirements.md`](2_SRS_Part3_Requirements.md) | Yêu cầu phi chức năng, **114 business rules**, danh sách thông điệp |
| [`api-endpoints-full.md`](api-endpoints-full.md) | Danh sách toàn bộ endpoint theo use case |
| [`../postman/`](../postman/) | Bộ Postman 99 request để chạy thử toàn bộ API |
| [`../README.md`](../README.md) | Kiến trúc, hướng dẫn cài đặt và chạy |
