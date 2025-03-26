import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-testlist',
  templateUrl: './testlist.component.html',
  styleUrls: ['./testlist.component.scss']
})
export class TestListComponent implements OnInit {
  statusList = ['Đã kiểm tra', 'Chưa kiểm tra'];
  classList = ['Lớp 6A1', 'Lớp 6A2', 'Lớp 7A1', 'Lớp 9B'];

  selectedStatus = '';
  selectedClass = '';
  searchText = '';

  testList = [ // Danh sách đầy đủ
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },
    {
      name: 'Bài kiểm tra chất lượng giữa kì',
      class: 'Ôn luyện cấp 3',
      createdDate: '23-08-2023',
      testDate: '30-09-2023',
      document: { name: 'toan_hinh_de2022.pdf', url: '#' },
      status: 'Đã kiểm tra',
      score: 'Download'
    },

    // Thêm nhiều dữ liệu để thử phân trang
  ];

  // Phân trang
  currentPage = 1;
  itemsPerPage = 5;
  paginatedList = [];

  showCreateForm = false;

  newTest = {
    name: '',
    testDate: '',
    class: '',
    document: null as any
  };
  showEditForm = false;
  editTest: any = {
    name: '',
    testDate: '',
    class: '',
    document: {
      name: '',
      url: ''
    }
  };

  ngOnInit() {
    this.updatePaginatedList();
  }

  updatePaginatedList() {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    this.paginatedList = this.testList.slice(start, end);
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages.length) return;
    this.currentPage = page;
    this.updatePaginatedList();
  }

  get totalPages(): number[] {
    return Array(Math.ceil(this.testList.length / this.itemsPerPage))
      .fill(0)
      .map((_, i) => i + 1);
  }

  getFileIcon(fileName: string): string {
    if (fileName.endsWith('.pdf')) return 'fa fa-file-pdf';
    if (fileName.endsWith('.zip')) return 'fa fa-file-archive';
    if (fileName.endsWith('.doc') || fileName.endsWith('.docx') || fileName.endsWith('.docs')) return 'fa fa-file-word';
    return 'fa fa-file';
  }

  // ===============================
  // TẠO BÀI KIỂM TRA
  // ===============================
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const fileData = {
        name: file.name,
        url: URL.createObjectURL(file)
      };

      // Nếu đang mở form tạo
      if (this.showCreateForm) {
        this.newTest.document = fileData;
      }

      // Nếu đang mở form sửa
      if (this.showEditForm) {
        this.editTest.document = fileData;
      }
    }
  }

  addTest() {
    if (!this.newTest.name || !this.newTest.testDate || !this.newTest.class || !this.newTest.document) {
      alert('Vui lòng điền đầy đủ thông tin!');
      return;
    }

    this.testList.unshift({
      name: this.newTest.name,
      class: this.newTest.class,
      createdDate: new Date().toLocaleDateString('vi-VN'),
      testDate: this.newTest.testDate,
      document: this.newTest.document,
      status: 'Chưa kiểm tra',
      score: ''
    });

    this.newTest = { name: '', testDate: '', class: '', document: null };
    this.showCreateForm = false;
    this.currentPage = 1;
    this.updatePaginatedList();
  }

  openModal() {
    this.showCreateForm = true;
    this.showEditForm = false; // tắt modal sửa khi mở modal tạo
  }
  

  closeModal() {
    console.log('Modal đóng ❌');
    this.showCreateForm = false;
  }

  // ===============================
  // SỬA BÀI KIỂM TRA
  // ===============================
  openEditModal(test: any) {
    this.editTest = { ...test };
    this.showEditForm = true;
    this.showCreateForm = false; // tắt modal tạo khi mở modal sửa
  }

  closeEditModal() {
    this.showEditForm = false;
  }

  updateTest() {
    const index = this.testList.findIndex(t => t === this.editTest);
    if (index > -1) {
      this.testList[index] = {
        ...this.editTest,
        createdDate: this.testList[index].createdDate // giữ nguyên ngày tạo
      };
    }

    this.showEditForm = false;
    this.updatePaginatedList();
  }
  removeDocument() {
    this.editTest.document = null;
  }
}
