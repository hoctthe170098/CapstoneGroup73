import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../auth/auth-guard.service';
import { UserRole } from 'app/pages/content-pages/shared/user.model';

//Route for content layout with sidebar, navbar and footer.

export const Full_ROUTES: Routes = [
  {
    path: 'phong',
    loadChildren: () => import('../../slot/slot.module').then(m => m.SlotModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.CampusManager]}
  },
  {
    path: 'chinhsach',
    loadChildren: () => import('../../chinhsach/chinhsach.module').then(m => m.ChinhsachModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Administrator]}
  },
  {
    path: 'baikiemtra',
    loadChildren: () => import('../../baikiemtra/baikiemtra.module').then(m => m.TestlistModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.LearningManager]}
  },
  {
    path: 'lichday',
    loadChildren: () => import('../../lichday/lichday.module').then(m => m.LichdayModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Teacher]}
  },
  {
    path: 'hocsinh',
    loadChildren: () => import('../../hocsinh/hocsinh.module').then(m => m.HocSinhModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.CampusManager]}
  },
  {
    path: 'quanly',
    loadChildren: () => import('../../quanly/quanly.module').then(m => m.AccountmanagerModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Administrator]}
  },
  {
    path: 'giaovien',
    loadChildren: () => import('../../giaovien/giaovien.module').then(m => m.GiaovienModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.CampusManager]}
  },
  {
    path: 'lophoc',
    loadChildren: () => import('../../lophoc/lophoc.module').then(m => m.LophocModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.CampusManager]}
  },
  {
    path: 'lopdangday',
    loadChildren: () => import('../../lopdangday/lopdangday.module').then(m => m.LopdangdayModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Teacher]}
  },
  {
    path: 'lopdanghoc',
    loadChildren: () => import('../../lopdanghoc/lopdanghoc.module').then(m => m.LopdanghocModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Student]}
  },
  {
    path: 'coso',
    loadChildren: () => import('../../coso/coso.module').then(m => m.CosoModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Administrator]}
  },
  {
    path: 'chuongtrinh',
    loadChildren: () => import('../../chuongtrinh/chuongtrinh.module').then(m => m.ChuongtrinhModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.LearningManager]}
  },
  {
    path: 'pages',
    loadChildren: () => import('../../pages/full-pages/full-pages.module').then(m => m.FullPagesModule),
    canActivate:[AuthGuard]
  },
  {
    path: 'lichhoc',
    loadChildren: () => import('../../lichhoc/lichhoc.module').then(m => m.LichhocModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Student]}
  },
  {
    path: 'dashboard-cm',
    loadChildren: () => import('../../dashboard-cm/dashboard-cm.module').then(m => m.DashboardCMModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.CampusManager]}
  },
  {
    path: 'dashboard-admin',
    loadChildren: () => import('../../dashboard-admin/dashboard-admin.module').then(m => m.DashboardAdminModule),
    canActivate:[AuthGuard],
    data:{role:[UserRole.Administrator]}
  }
];
