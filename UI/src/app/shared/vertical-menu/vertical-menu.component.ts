import {
  Component, OnInit, ViewChild, OnDestroy,
  ElementRef, AfterViewInit, ChangeDetectorRef, HostListener
} from "@angular/core";
import { ROUTES, RouteInfo } from './vertical-menu-routes.config'; // Import RouteInfo
import { HROUTES } from '../horizontal-menu/navigation-routes.config';

import { Router } from "@angular/router";
import { TranslateService } from '@ngx-translate/core';
import { customAnimations } from "../animations/custom-animations";
import { DeviceDetectorService } from 'ngx-device-detector';
import { ConfigService } from '../services/config.service';
import { Subscription } from 'rxjs';
import { LayoutService } from '../services/layout.service';
import { Subject } from 'rxjs';
import { UserService } from "app/pages/content-pages/shared/user.service";
import { UserRole } from "app/pages/content-pages/shared/user.model";

@Component({
  selector: "app-sidebar",
  templateUrl: "./vertical-menu.component.html",
  animations: customAnimations
})
export class VerticalMenuComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('toggleIcon') toggleIcon: ElementRef;
  public menuItems: RouteInfo[] = []; // Sử dụng RouteInfo[]
  level: number = 0;
  logoUrl = 'assets/img/studyflow_logo_icon.png';
  public config: any = {};
  protected innerWidth: any;
  layoutSub: Subscription;
  configSub: Subscription;
  perfectScrollbarEnable = true;
  collapseSidebar = false;
  resizeTimeout;
  userRoles: string[] | null = null;
  private ngUnsubscribe = new Subject<void>();

  constructor(
    private authService: UserService,
    private router: Router,
    public translate: TranslateService,
    private layoutService: LayoutService,
    private configService: ConfigService,
    private cdr: ChangeDetectorRef,
    private deviceService: DeviceDetectorService,
    private userService: UserService // Inject UserService
  ) {
    this.config = this.configService.templateConf;
    this.innerWidth = window.innerWidth;
    this.isTouchDevice();
  }


  ngOnInit() {
    this.userRoles = this.userService.getRoleNames();
    this.filterMenuItems();

    // (Tùy chọn) Theo dõi thay đổi vai trò nếu cần
    // this.userService.userRolesChanged$
    //   .pipe(takeUntil(this.ngUnsubscribe))
    //   .subscribe(() => this.filterMenuItems());
  }

  ngAfterViewInit() {
    this.configSub = this.configService.templateConf$.subscribe((templateConf) => {
      if (templateConf) {
        this.config = templateConf;
      }
      this.loadLayout();
      this.cdr.markForCheck();
    });

    this.layoutSub = this.layoutService.overlaySidebarToggle$.subscribe(
      collapse => {
        if (this.config.layout.menuPosition === "Side") {
          this.collapseSidebar = collapse;
        }
      });
  }


  @HostListener('window:resize', ['$event'])
  onWindowResize(event) {
      if (this.resizeTimeout) {
          clearTimeout(this.resizeTimeout);
      }
      this.resizeTimeout = setTimeout((() => {
        this.innerWidth = event.target.innerWidth;
          this.loadLayout();
      }).bind(this), 500);
  }

  loadLayout() {
    this.logoUrl = 'assets/img/studyflow_logo_icon.png';
  }

  toggleSidebar() {
    let conf = this.config;
    conf.layout.sidebar.collapsed = !this.config.layout.sidebar.collapsed;
    this.configService.applyTemplateConfigChange({ layout: conf.layout });

    setTimeout(() => {
      this.fireRefreshEventOnWindow();
    }, 300);
  }

  fireRefreshEventOnWindow = function () {
    const evt = document.createEvent("HTMLEvents");
    evt.initEvent("resize", true, false);
    window.dispatchEvent(evt);
  };

  CloseSidebar() {
    this.layoutService.toggleSidebarSmallScreen(false);
  }

  isTouchDevice() {
    const isMobile = this.deviceService.isMobile();
    const isTablet = this.deviceService.isTablet();

    if (isMobile || isTablet) {
      this.perfectScrollbarEnable = false;
    }
    else {
      this.perfectScrollbarEnable = true;
    }
  }

  filterMenuItems(): void {
    if (this.userRoles) {
      this.menuItems = ROUTES.filter(item => this.checkPermission(item));
    } else {
      // Nếu chưa đăng nhập hoặc không có vai trò, hiển thị một số menu mặc định (nếu cần)
      this.menuItems = ROUTES.filter(item => item.path === '/dashboard' || item.path.startsWith('/pages/'));
    }
  }

  checkPermission(routeInfo: RouteInfo): boolean {
    if (!routeInfo.roles || routeInfo.roles.length === 0) {
      return true; // Không có roles cụ thể, cho phép tất cả
    }
    if (!this.userRoles) {
      return false; // Chưa đăng nhập, không cho phép
    }
    return this.userRoles.some(role => routeInfo.roles!.includes(role));
  }

  ngOnDestroy() {
    if (this.layoutSub) {
      this.layoutSub.unsubscribe();
    }
    if (this.configSub) {
      this.configSub.unsubscribe();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  clickLogo(){
    var role = this.authService.getRoleNames()[0];
    if(role==UserRole.Administrator)
    this.router.navigate(['/coso']);
  else if(role==UserRole.CampusManager){
    this.router.navigate(['/lophoc']);
  }else if(role==UserRole.LearningManager){
    this.router.navigate(['/chuongtrinh']);
  }else if(role==UserRole.Teacher){
    this.router.navigate(['/lopdangday']);
  }else if(role==UserRole.Student){
    this.router.navigate(['/lopdanghoc']);
  }else this.router.navigate(['/'])
  }
}