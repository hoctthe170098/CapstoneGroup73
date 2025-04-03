import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { LophocService } from "./shared/lophoc.service";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { NgxSpinnerService } from "ngx-spinner";
@Component({
  selector: "app-lophoc",
  templateUrl: "./lophoc.component.html",
  styleUrls: ["./lophoc.component.scss"],
})
export class LophocComponent implements OnInit {
  thuTrongTuan = {
    thu2: false,
    thu3: false,
    thu4: false,
    thu5: false,
    thu6: false,
    thu7: false,
    cn: false,
  };

  chuongTrinh: string = "";
  trangThai: string = "";
  timeStart: string = "";
  timeEnd: string = "";
  dateStart: string = "";
  dateEnd: string = "";
  searchTerm: string = "";
  chuongTrinhOptions: any[] = [];
  lophocs: any[] = [];
  chuongTrinhId: number = 0;
  currentPage: number = 1;
  totalPages: number = 1;
  pageSize: number = 3;

  giaoVienSearch: string = "";
  giaoVienOptions: { code: string; codeTen: string }[] = [];
  filteredGiaoVienOptions: { code: string; codeTen: string }[] = [];
  selectedGiaoVien: string = "";
  isGiaoVienDropdownOpen: boolean = false;
  phongOptions: { id: string; ten: string }[] = [];
  popupGiaoVienOptions: { code: string; codeTen: string }[] = [];
  popupFilteredGiaoVienOptions: { code: string; codeTen: string }[] = [];
  popupGiaoVienSearch: string = "";
  isPopupGiaoVienDropdownOpen: boolean = false;
  isAddScheduleModalOpen: boolean = false;
  showExtraFields: boolean = false;

  isEditScheduleModalOpen: boolean = false;
editingLichDayThay: any = null; 
isPopupGiaoVienDropdownOpen_Edit: boolean = false;
popupFilteredGiaoVienOptions_Edit: { code: string, codeTen: string }[] = [];

  constructor(
    private lophocService: LophocService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit(): void {
    this.getChuongTrinhs();
    this.loadLopHocs();
    this.getGiaoVienOptions();
    document.addEventListener("click", (event: any) => {
      const isInside = event.target.closest(".action-plus-wrapper");
      if (!isInside) {
        this.closeAllActionMenus();
      }
    });
  }
  toggleActionMenu(index: number, event: MouseEvent) {
    event.stopPropagation();
    this.lophocs.forEach((_, i) => {
      this.lophocs[i].showActionMenu =
        i === index ? !this.lophocs[i].showActionMenu : false;
    });
  }
  handleClickOutsidePopupDropdown(event: any) {
    const dropdown = document.querySelector('.popup-giaovien-dropdown-wrapper');
    if (dropdown && !dropdown.contains(event.target as Node)) {
      this.isPopupGiaoVienDropdownOpen = false;
    }
  }
  closeAllActionMenus() {
    this.lophocs.forEach((l) => (l.showActionMenu = false));
  }

  onActionClick(action: string, lop: any) {
    this.closeAllActionMenus();
    alert(`Bạn đã chọn: ${action} cho lớp ${lop.tenLop}`);
    // TODO: điều hướng hoặc mở form tương ứng
  }

  newSchedule = {
    loaiLich: "",
    ngay: "",
    ngayNghi: "", 
    phong: "",
    batDau: "",
    ketThuc: "",
    giaoVienCode: "",
    lop: null,
  };
  openAddScheduleModal(lop: any, loai: string) {
    this.newSchedule = {
      loaiLich: loai,
      lop: lop,
      ngay: '',
      ngayNghi: '',
      phong: '',
      batDau: '',
      ketThuc: '',
       giaoVienCode: ''
    };
    this.isAddScheduleModalOpen = true;
    this.showExtraFields = false;
  
    if (loai === 'Dạy bù') {
      this.loadPhongs();
    } else if (loai === 'Dạy thay') {
      this.loadPopupGiaoViens(); 
    }
  }

  closeAddScheduleModal() {
    this.isAddScheduleModalOpen = false;
  }

  loadPhongs() {
    this.lophocService.getPhongs().subscribe({
      next: (res) => {
        this.phongOptions = !res.isError && res.data ? res.data : [];
      },
      error: () => {
        this.phongOptions = [];
      },
    });
  }
  // Giao viên trong popup
  loadPopupGiaoViens() {
    this.lophocService.getGiaoViens({ searchTen: "" }).subscribe({
      next: (res) => {
        this.popupGiaoVienOptions = res.data || [];
        this.popupFilteredGiaoVienOptions = res.data.slice();
      },
      error: () => {
        this.popupGiaoVienOptions = [];
        this.popupFilteredGiaoVienOptions = [];
      },
    });
  }

  togglePopupGiaoVienDropdown(event: MouseEvent) {
    event.stopPropagation();
    this.isPopupGiaoVienDropdownOpen = !this.isPopupGiaoVienDropdownOpen;
  }
  togglePopupGiaoVienDropdown_Edit(event: Event) {
    event.stopPropagation();
    this.isPopupGiaoVienDropdownOpen_Edit = !this.isPopupGiaoVienDropdownOpen_Edit;
  }

  getSelectedPopupGiaoVienText(): string {
    const selected = this.popupGiaoVienOptions.find(
      (gv) => gv.code === this.newSchedule.giaoVienCode
    );
    return selected ? selected.codeTen : "";
  }

  onPopupGiaoVienSearch() {
    const search = this.popupGiaoVienSearch.toLowerCase();
    this.popupFilteredGiaoVienOptions = this.popupGiaoVienOptions.filter((gv) =>
      gv.codeTen.toLowerCase().includes(search)
    );
  }
  onPopupGiaoVienSearch_Edit() {
    const search = this.popupGiaoVienSearch.toLowerCase();
    this.popupFilteredGiaoVienOptions_Edit = this.giaoVienOptions.filter(gv =>
      gv.codeTen.toLowerCase().includes(search)
    );
  }
  selectPopupGiaoVien_Edit(gv: { code: string, codeTen: string }) {
    this.editingLichDayThay.giaoVienCode = gv.code;
    this.popupGiaoVienSearch = '';
    this.popupFilteredGiaoVienOptions_Edit = this.giaoVienOptions.slice();
    this.isPopupGiaoVienDropdownOpen_Edit = false;
  }
  selectPopupGiaoVien(gv: { code: string, codeTen: string }) {
    this.newSchedule.giaoVienCode = gv.code;
    this.popupGiaoVienSearch = ''; 
    this.popupFilteredGiaoVienOptions = this.popupGiaoVienOptions.slice(); 
    this.isPopupGiaoVienDropdownOpen = false; 
  }
  

  submitNewSchedule() {
    if (this.newSchedule.loaiLich === 'Dạy thay') {
      const payload = {
        tenLop: this.newSchedule.lop?.tenLop,
        ngayDay: this.newSchedule.ngay,
        giaoVienCode: this.newSchedule.giaoVienCode
      };
  
      this.lophocService.createLichDayThay(payload).subscribe({
        next: (res) => {
          if (!res.isError) {
            this.toastr.success(res.message || 'Thêm lịch dạy thay thành công!');
            this.closeAddScheduleModal();
            this.loadLopHocs(); 
          } else {
            this.toastr.error(res.message || 'Thêm lịch dạy thay thất bại!');
          }
        },
        error: (err) => {
          console.error('Lỗi khi tạo lịch dạy thay:', err);
          this.toastr.error('Không thể tạo lịch dạy thay!');
        }
      });
    } else {
      console.log("Thêm lịch dạy bù:", this.newSchedule);
      this.toastr.success("Đã thêm lịch dạy bù!");
      this.closeAddScheduleModal();
    }
  }

  getChuongTrinhs(): void {
    this.lophocService.getChuongTrinhs().subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.chuongTrinhOptions = res.data;
        } else {
          this.chuongTrinhOptions = [];
        }
      },
      error: (err) => {
        console.error("Lỗi khi lấy chương trình:", err);
        this.chuongTrinhOptions = [];
      },
    });
  }
  getGiaoVienOptions() {
    const payload = { searchTen: "" };
    this.lophocService.getGiaoViens(payload).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.giaoVienOptions = res.data;
          this.filteredGiaoVienOptions = res.data.slice();
        } else {
          this.giaoVienOptions = [];
          this.filteredGiaoVienOptions = [];
        }
      },
      error: (err) => {
        console.error("Lỗi khi lấy giáo viên:", err);
      },
    });
  }
  getThuHienThi(thu: number): string {
    const thuMap: { [key: number]: string } = {
      2: "Thứ 2",
      3: "Thứ 3",
      4: "Thứ 4",
      5: "Thứ 5",
      6: "Thứ 6",
      7: "Thứ 7",
      8: "Chủ Nhật",
    };
    return thuMap[thu] || `Thứ ${thu}`;
  }

  getSelectedGiaoVienText(): string {
    const selected = this.giaoVienOptions.find(
      (gv) => gv.code === this.selectedGiaoVien
    );
    return selected ? selected.codeTen : "";
  }
  clearGiaoVienSelection(event: MouseEvent) {
    event.stopPropagation();
    this.selectedGiaoVien = "";
    this.giaoVienSearch = "";
    this.filteredGiaoVienOptions = this.giaoVienOptions.slice();
    this.currentPage = 1;
    this.loadLopHocs();
  }

  onGiaoVienSearch() {
    const searchTerm = this.giaoVienSearch.toLowerCase();
    this.filteredGiaoVienOptions = this.giaoVienOptions.filter((gv) =>
      gv.codeTen.toLowerCase().includes(searchTerm)
    );
  }

  selectGiaoVien(gv: { code: string; codeTen: string }) {
    this.selectedGiaoVien = gv.code;
    this.giaoVienSearch = "";
    this.isGiaoVienDropdownOpen = false;
    this.currentPage = 1;
    this.loadLopHocs();
  }

  toggleGiaoVienDropdown() {
    this.isGiaoVienDropdownOpen = !this.isGiaoVienDropdownOpen;
  }

  loadLopHocs(page: number = this.currentPage) {
    const thus: number[] = [];
    if (this.thuTrongTuan.thu2) thus.push(2);
    if (this.thuTrongTuan.thu3) thus.push(3);
    if (this.thuTrongTuan.thu4) thus.push(4);
    if (this.thuTrongTuan.thu5) thus.push(5);
    if (this.thuTrongTuan.thu6) thus.push(6);
    if (this.thuTrongTuan.thu7) thus.push(7);
    if (this.thuTrongTuan.cn)  thus.push(8);
  
    const payload = {
      pageNumber: page,
      pageSize: this.pageSize,
      tenLop: this.searchTerm,
      thus: thus,
      giaoVienCode: this.selectedGiaoVien || "all",
      phongId: 0,
      chuongTrinhId: this.chuongTrinhId || 0,
      trangThai: this.trangThai || "all",
      thoiGianBatDau: this.timeStart || "",
      thoiGianKetThuc: this.timeEnd || "",
      ngayBatDau: this.dateStart || "0001-01-01",
      ngayKetThuc: this.dateEnd || "0001-01-01",
    };
  
    this.spinner.show();
    this.lophocService.getDanhSachLopHoc(payload).subscribe({
      next: (response) => {
        this.spinner.hide();
        if (!response.isError && response.data) {
          const data = response.data;
  
          this.lophocs = data.lopHocs.map((item: any) => {
            const coDinh = item.loaiLichHocs.find((l: any) => l.trangThai === 'Cố định')?.lichHocs || [];
            const dayBu = item.loaiLichHocs.find((l: any) => l.trangThai === 'Dạy bù')?.lichHocs || [];
            const dayThay = item.loaiLichHocs.find((l: any) => l.trangThai === 'Dạy thay')?.lichHocs || [];
            const daNghi = item.loaiLichHocs.find((l: any) => l.trangThai === 'Đã nghỉ')?.lichHocs || [];
            const firstLich = coDinh[0];
  
            return {
              ...item,
              chuongTrinh: item.tenChuongTrinh,
              phong: firstLich?.tenPhong || "",
              giaoVien: item.tenGiaoVien,
              ngayBatDau: firstLich?.ngayBatDau,
              ngayKetThuc: firstLich?.ngayKetThuc,
              hocPhi: item.hocPhi,
              trangThai: item.loaiLichHocs.find((l: any) => l.lichHocs.length > 0)?.trangThai || "Không xác định",
  
              lichCoDinh: coDinh.map((lh: any) => ({ ...lh })), 
              lichDayBu: dayBu.map((lh: any) => ({ ...lh })),
              lichDayThay: dayThay.map((lh: any) => ({
                ...lh,
                giaoVienCode: lh.giaoVienCode || '',
                tenGiaoVien: lh.tenGiaoVien || ''
              })),
              lichNghi: daNghi.map((lh: any) => ({ ...lh }))
            };
          });
  
          this.currentPage = data.pageNumber;
          this.totalPages = Math.ceil(data.totalCount / this.pageSize);
          this.cdr.detectChanges();
        } else {
          this.lophocs = [];
          this.totalPages = 1;
        }
      },
      error: (err) => {
        this.spinner.hide();
        console.error("Lỗi khi lấy danh sách lớp học:", err);
        this.lophocs = [];
        this.totalPages = 1;
      },
    });
  }
  

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadLopHocs(page);
  }

  onAddClass() {
    this.router.navigate(["/lophoc/add"]);
  }

  onEditClass(index: number): void {
    const tenLop = this.lophocs[index].tenLop;
    console.log('🚀 Tên lớp từ UI:', tenLop);
  
    this.lophocService.getLopHocByTenLop(tenLop).subscribe(
      (res) => {
        console.log('✅ Kết quả API:',tenLop);
        this.router.navigate(['/lophoc/edit', tenLop]);

      },
      (err) => {
        console.error('❌ Lỗi gọi API:', err);
      }
    );
  }
  
  
  onFilterChange() {
    this.currentPage = 1;
    this.loadLopHocs();
  }
  
  
  onDeleteLichDayThay(lich: any) {
    console.log(" Xóa lịch dạy thay:", lich);
  }
  
  onEditLichDayThay(lich: any, lop: any) {
    console.log("GV code:", lich.giaoVienCode);
    console.log("Lịch dạy thay được chọn để sửa:", lich);
    const matchedGV = this.giaoVienOptions.find(gv => gv.codeTen === lich.tenGiaoVien);
  
    this.editingLichDayThay = {
      ...lich,
      tenLop: lop.tenLop,
      giaoVienCode: lich.giaoVienCode || '',
    };
  
    this.popupGiaoVienSearch = '';
    this.popupFilteredGiaoVienOptions_Edit = this.giaoVienOptions.slice(); // ← clone cho edit
    this.isPopupGiaoVienDropdownOpen_Edit = false;
    
    this.isEditScheduleModalOpen = true;
  }
  
  closeEditScheduleModal() {
    this.isEditScheduleModalOpen = false;
    this.editingLichDayThay = null;
  }
  submitEditLichDayThay() {
    console.log(" Chỉnh sửa lịch dạy thay:", this.editingLichDayThay);
    this.toastr.success("Đã cập nhật lịch dạy thay!");
    this.closeEditScheduleModal();
  }
}
