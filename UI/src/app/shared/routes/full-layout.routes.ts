import { Routes, RouterModule } from '@angular/router';

//Route for content layout with sidebar, navbar and footer.

export const Full_ROUTES: Routes = [
  {
    path: 'dashboard',
    loadChildren: () => import('../../dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  {
    path: 'sanpham',
    loadChildren: () => import('../../sanpham/sanpham.module').then(m => m.SanphamModule)
  },
  
  {
    path: 'phong',
    loadChildren: () => import('../../slot/slot.module').then(m => m.SlotModule)
  },
  {
    path: 'chinhsach',
    loadChildren: () => import('../../chinhsach/chinhsach.module').then(m => m.ChinhsachModule)
  },
  {
    path: 'baikiemtra',
    loadChildren: () => import('../../baikiemtra/baikiemtra.module').then(m => m.TestlistModule)
  },
  {
    path: 'lichday',
    loadChildren: () => import('../../lichday/lichday.module').then(m => m.LichdayModule)
  },
  {
    path: 'hocsinh',
    loadChildren: () => import('../../hocsinh/hocsinh.module').then(m => m.HocSinhModule)
  },
  {
    path: 'quanly',
    loadChildren: () => import('../../quanly/quanly.module').then(m => m.AccountmanagerModule)
  },
  {
    path: 'giaovien',
    loadChildren: () => import('../../giaovien/giaovien.module').then(m => m.GiaovienModule)
  },
  {
    path: 'lophoc',
    loadChildren: () => import('../../lophoc/lophoc.module').then(m => m.LophocModule)
  },
  {
    path: 'lopdangday',
    loadChildren: () => import('../../lopdangday/lopdangday.module').then(m => m.LopdangdayModule)
  },
  {
    path: 'lopdanghoc',
    loadChildren: () => import('../../lopdanghoc/lopdanghoc.module').then(m => m.LopdanghocModule)
  },
  {
    path: 'coso',
    loadChildren: () => import('../../coso/coso.module').then(m => m.CosoModule)
  },
  {
    path: 'chuongtrinh',
    loadChildren: () => import('../../chuongtrinh/chuongtrinh.module').then(m => m.ChuongtrinhModule)
  },
  {
    path: 'calendar',
    loadChildren: () => import('../../calendar/calendar.module').then(m => m.CalendarsModule)
  },
  {
    path: 'charts',
    loadChildren: () => import('../../charts/charts.module').then(m => m.ChartsNg2Module)
  },
   {
    path: 'forms',
    loadChildren: () => import('../../forms/forms.module').then(m => m.FormModule)
  },
  {
    path: 'maps',
    loadChildren: () => import('../../maps/maps.module').then(m => m.MapsModule)
  },
  {
    path: 'tables',
    loadChildren: () => import('../../tables/tables.module').then(m => m.TablesModule)
  },
  {
    path: 'datatables',
    loadChildren: () => import('../../data-tables/data-tables.module').then(m => m.DataTablesModule)
  },
  {
    path: 'uikit',
    loadChildren: () => import('../../ui-kit/ui-kit.module').then(m => m.UIKitModule)
  },
  {
    path: 'components',
    loadChildren: () => import('../../components/ui-components.module').then(m => m.UIComponentsModule)
  },
  {
    path: 'pages',
    loadChildren: () => import('../../pages/full-pages/full-pages.module').then(m => m.FullPagesModule)
  },
  {
    path: 'cards',
    loadChildren: () => import('../../cards/cards.module').then(m => m.CardsModule)
  },
  {
    path: 'chat',
    loadChildren: () => import('../../chat/chat.module').then(m => m.ChatModule)
  },
  {
    path: 'chat-ngrx',
    loadChildren: () => import('../../chat-ngrx/chat-ngrx.module').then(m => m.ChatNGRXModule)
  },
  {
    path: 'inbox',
    loadChildren: () => import('../../inbox/inbox.module').then(m => m.InboxModule)
  },
  {
    path: 'taskboard',
    loadChildren: () => import('../../taskboard/taskboard.module').then(m => m.TaskboardModule)
  },
  {
    path: 'taskboard-ngrx',
    loadChildren: () => import('../../taskboard-ngrx/taskboard-ngrx.module').then(m => m.TaskboardNGRXModule)
  },
  {
    path: 'baocaohocphi',
    loadChildren: () => import('../../baocaohocphi/baocaohocphi.module').then(m => m.BaocaohocphiModule)
  },
  {
    path: 'baocaodiemdanhquanlycoso',
    loadChildren: () => import('../../baocaodiemdanhquanlycoso/baocaodiemdanhquanlycoso.module').then(m => m.BaocaodiemdanhquanlycosoModule)
  },
  {
    path: 'lichhoc',
    loadChildren: () => import('../../lichhoc/lichhoc.module').then(m => m.LichhocModule)
  },

];
