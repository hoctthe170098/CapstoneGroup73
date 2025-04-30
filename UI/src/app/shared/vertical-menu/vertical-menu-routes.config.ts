import { UserRole } from 'app/pages/content-pages/shared/user.model';
import { RouteInfo } from './vertical-menu.metadata';

//Sidebar menu Routes and data
export const ROUTES: RouteInfo[] = [
  {path: '/dashboard-admin', title: 'Dashboard', icon: 'ft-home', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [],roles:[UserRole.Administrator] },
  {path: '/dashboard-cm', title: 'Dashboard', icon: 'ft-home', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [],roles:[UserRole.CampusManager] },
  {path: '/phong', title: 'Phòng Học', icon: 'ft-briefcase', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.CampusManager] },
  {path: '/chuongtrinh', title: 'Chương Trình', icon: 'ft-book', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [],roles:[UserRole.LearningManager] },
  {path: '/lophoc', title: 'Lớp Học', icon: 'ft-book-open', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.CampusManager] },
  {path: '/lopdangday', title: 'Lớp Đang Dạy', icon: 'ft-book-open', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.Teacher] },
  {path: '/lopdanghoc', title: 'Lớp Đang Học', icon: 'ft-book-open', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [],roles:[UserRole.Student]  },
  {path: '/chinhsach', title: 'Chính Sách', icon: 'ft-sidebar', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [],roles:[UserRole.Administrator]  },
  {path: '/quanly', title: 'Quản Lý', icon: 'ft-user', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.Administrator] },
  {path: '/lichday', title: 'Lịch Dạy', icon: 'ft-calendar', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [],roles:[UserRole.Teacher]  },
  {path: '/lichhoc', title: 'Lịch Học', icon: 'ft-calendar', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.Student] },
  {path: '/baikiemtra', title: 'Bài Kiểm Tra', icon: 'ft-file-text', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.LearningManager] },
  {path: '/giaovien', title: 'Giáo Viên', icon: 'ft-user', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.CampusManager] },
  {path: '/hocsinh', title: 'Học Sinh', icon: 'ft-user', class: '', badge: '', badgeClass: '', isExternalLink: false, submenu: [] ,roles:[UserRole.CampusManager] },
  { path: '/coso/danh-sach-coso', title: 'Cơ Sở', icon: 'ft-home', class: 'dropdown-item', isExternalLink: false, submenu: [] ,roles:[UserRole.Administrator] }
];
export { RouteInfo };

