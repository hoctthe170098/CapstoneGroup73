import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { saveAs } from "file-saver";
import { LopdangdayService } from "../shared/lopdangday.service";
import { ChuongtrinhService } from "app/chuongtrinh/shared/chuongtrinh.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-chuongtrinhhoc",
  templateUrl: "./chuongtrinhhoc.component.html",
  styleUrls: ["./chuongtrinhhoc.component.scss"],
})
export class ChuongtrinhhocComponent implements OnInit {
  chuongTrinhs: any = null;
  tenLop: string = "";

  constructor(
    private lopDangHocService: LopdangdayService,
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
        } else {
          if(response.code === 404)
          this.router.navigate(["/pages/error"]);
          else this.toastr.error(response.message)
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
        }
      },
      (error: any) => {
        // Trường hợp lỗi HTTP
        this.toastr.error("Đã có lỗi xảy ra.");
      }
    );
  }
}
