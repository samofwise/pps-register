type PersonalPropertySecurity = {
  id: number;
  grantorFirstName: string;
  grantorMiddleNames?: string;
  grantorLastName: string;
  vin: string;
  registrationStartDate: Date;
  registrationDuration: string;
  spgAcn: string;
  spgOrganizationName: string;
};

export default PersonalPropertySecurity;