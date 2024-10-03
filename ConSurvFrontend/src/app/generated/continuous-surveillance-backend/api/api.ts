export * from './camera.service';
import { CameraService } from './camera.service';
export * from './commonRoutes.service';
import { CommonRoutesService } from './commonRoutes.service';
export * from './maintenanceRoutes.service';
import { MaintenanceRoutesService } from './maintenanceRoutes.service';
export * from './user.service';
import { UserService } from './user.service';
export const APIS = [CameraService, CommonRoutesService, MaintenanceRoutesService, UserService];
