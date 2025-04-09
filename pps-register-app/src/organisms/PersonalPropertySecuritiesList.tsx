import { Fragment } from 'react/jsx-runtime';
import personalPropertySecuritiesApi from '../api/personalPropertySecuritiesApi';
import LoaderWrapper from '../molecules/LoaderWrapper';
import { ArrowPathIcon } from '@heroicons/react/24/outline';

const PersonalPropertySecuritiesList = ({
  className,
}: {
  className?: string;
}) => {
  const { data: personalPropertySecurities, isFetching } =
    personalPropertySecuritiesApi.useGetPersonalPropertySecurities();

  const invalidatePersonalPropertySecurities =
    personalPropertySecuritiesApi.useInvalidatePersonalPropertySecurities();

  return (
    <article className={className}>
      <h2 className="text-2xl font-semibold mb-4">
        Personal Property Securities{' '}
        <ArrowPathIcon
          className="inline-block cursor-pointer size-6"
          onClick={invalidatePersonalPropertySecurities}
        />
      </h2>
      <LoaderWrapper loading={isFetching}>
        <div className="overflow-x-scroll w-full">
          <section className="grid grid-cols-[auto_auto_auto_auto_auto_auto_auto_auto] min-h-10 border border-gray-200 rounded-2xl divide-x divide-y divide-gray-200">
            <h3 className="p-2 font-semibold whitespace-nowrap">
              Grantor First Name
            </h3>
            <h3 className="p-2 font-semibold whitespace-nowrap">
              Grantor Middle Names
            </h3>
            <h3 className="p-2 font-semibold whitespace-nowrap">
              Grantor Last Name
            </h3>
            <h3 className="p-2 font-semibold whitespace-nowrap">VIN</h3>
            <h3 className="p-2 font-semibold whitespace-nowrap">
              Registration Start Date
            </h3>
            <h3 className="p-2 font-semibold whitespace-nowrap">
              Registration Duration
            </h3>
            <h3 className="p-2 font-semibold whitespace-nowrap">SPG ACN</h3>
            <h3 className="p-2 font-semibold whitespace-nowrap">
              SPG Organization Name
            </h3>
            {personalPropertySecurities &&
              personalPropertySecurities?.map((pps) => (
                <Fragment key={pps.id}>
                  <p className="p-2 whitespace-nowrap">
                    {pps.grantorFirstName}
                  </p>
                  <p className="p-2 whitespace-nowrap">
                    {pps.grantorMiddleNames}
                  </p>
                  <p className="p-2 whitespace-nowrap">{pps.grantorLastName}</p>
                  <p className="p-2 whitespace-nowrap">{pps.vin}</p>
                  <p className="p-2 whitespace-nowrap">
                    {new Date(pps.registrationStartDate).toLocaleDateString()}
                  </p>
                  <p className="p-2 whitespace-nowrap">
                    {pps.registrationDuration}
                  </p>
                  <p className="p-2 whitespace-nowrap">{pps.spgAcn}</p>
                  <p className="p-2 whitespace-nowrap">
                    {pps.spgOrganizationName}
                  </p>
                </Fragment>
              ))}
            {personalPropertySecurities?.length === 0 && (
              <section className="col-span-8 p-2 text-gray-500">
                <p className="text-lg text-center">No uploads found</p>
              </section>
            )}
          </section>
        </div>
      </LoaderWrapper>
      <style>
      .scroll-container {
          display: grid;
          grid-auto-flow: column;
          grid-auto-columns: 300px;
          gap: 1rem;
          overflow-x: auto;
          padding: 1rem;
          scroll-snap-type: x mandatory;
        }

        .item {
          background: #eee;
          border-radius: 10px;
          padding: 2rem;
          scroll-snap-align: start;
          min-height: 200px;
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 1.5rem;
        }
      </style>

      <div className="scroll-container">
        <div className="item">Item 1</div>
        <div className="item">Item 2</div>
        <div className="item">Item 3</div>
        <div className="item">Item 4</div>
        <div className="item">Item 5</div>
        <div className="item">Item 6</div>
      </div>
    </article>
  );
};

export default PersonalPropertySecuritiesList;
