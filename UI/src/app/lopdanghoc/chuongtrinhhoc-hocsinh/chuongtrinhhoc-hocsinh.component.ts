import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { LopdanghocService } from "../shared/lopdanghoc.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { ChuongtrinhService } from "app/chuongtrinh/shared/chuongtrinh.service";
import { saveAs } from "file-saver";

@Component({
  selector: "app-chuongtrinhhoc-hocsinh",
  templateUrl: "./chuongtrinhhoc-hocsinh.component.html",
  styleUrls: ["./chuongtrinhhoc-hocsinh.component.scss"],
})
export class ChuongtrinhhocHocsinhComponent implements OnInit {
  chuongTrinhs: any = null;
  tenLop: string = "";

  constructor(
    private lopDangHocService: LopdanghocService,
    private chuongTrinhService: ChuongtrinhService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((params) => {
      this.tenLop = decodeURIComponent(params.get("tenLop") || "");
      this.loadDanhSachChuongTrinhHoc();
    });
  }

  loadDanhSachChuongTrinhHoc() {
    this.lopDangHocService.getChuongTrinhLopHoc(this.tenLop).subscribe(
      (response) => {
        if (!response.isError) {
          this.chuongTrinhs = response.data;
          console.debug("Danh sách chương trình:", this.chuongTrinhs);
        } else {
          if (response.message === "Dữ liệu không tồn tại!") {
            this.toastr.error("Id không hợp lê!", "Lỗi");
          } else response.code === 404;
          this.router.navigate(["/pages/error"]);
        }
        this.cdr.detectChanges();
      },
      (error) => {
        console.debug("Error fetching programs:", error);
      }
    );
  }

  downloadFile(fileUrl: string, fileName: string): void {
    this.chuongTrinhService.downloadFile(fileUrl).subscribe(
      (res: any) => {
        if (res instanceof Blob) {
          // Trường hợp thành công (nhận Blob)
          saveAs(res, fileName);
        } else if (res && res.isError) {
          // Trường hợp lỗi (nhận res JSON)
          this.toastr.error(res.message);
          console.debug("Error downloading file:", res.message);
        }
      },
      (error: any) => {
        // Trường hợp lỗi HTTP
        this.toastr.error("Đã có lỗi xảy ra.");
      }
    );
  }
}
