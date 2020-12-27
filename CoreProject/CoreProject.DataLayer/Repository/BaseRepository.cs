using CoreProject.DataLayer.Helpers;
using CoreProject.DataLayer.Helpers.Enum;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.LogService;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity, new()
    {
        private readonly string _tableName;
        public IUnitOfWork _unitOfWork { get; private set; }

        readonly Commander _nlog;
        readonly OrmHelper OrmHelper;
        readonly OrmSqlEnum OrmSqlEnum;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _tableName = typeof(T).Name;
            OrmHelper = new OrmHelper(typeof(T));
            OrmSqlEnum = new OrmSqlEnum();
            _nlog = new Commander(_unitOfWork.Context.ConnectionString);
        }

        public async Task<ServiceResponse<T>> GetAllAsync()
        {
            var response = new ServiceResponse<T>();
            try
            {
                response.List = await _unitOfWork.QueryAsync<T>(OrmSqlEnum.GetAllSQL<T>());

                if (response.List == null) throw new KeyNotFoundException($"{_tableName} Tablo'sunda hiç {ErrorCodes.KayitYok.Text}");

                response.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description: {response.ExceptionMessage}!", ex);
            }

            return response;

        }

        public async Task<ServiceResponse<T>> GetByIdAsync(int id)
        {
            if (id == 0) throw new ArgumentNullException("id", ErrorCodes.WrongParameter.Text);

            var response = new ServiceResponse<T>();
            try
            {
                response.Entity = await _unitOfWork.QueryFirstOrDefaultAsync<T>(OrmSqlEnum.GetByIdSQL<T>(), new Dictionary<string, object> { { "Id", id } });

                if (response.Entity == null) throw new KeyNotFoundException($"{_tableName} Tablosunda [{id}] Nolu {ErrorCodes.KayitYok.Text}");

                response.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName},  Method:{MethodBase.GetCurrentMethod().Name}, Description: {response.ExceptionMessage}!", ex);
            }
            return response;

        }

        public async Task<ServiceResponse<T>> GetByParamAsync(object param)
        {

            var response = new ServiceResponse<T>();
            try
            {
                var result = await _unitOfWork.GetByParamAsync<T>(param);

                if (result.Count() == 1) response.Entity = result.FirstOrDefault();

                else if (result.Count() > 1) response.List = result;

                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{param}] ile ilgili {ErrorCodes.KayitYok.Text}");

                response.IsSuccessful = true;

            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description: {response.ExceptionMessage}!", ex);
            }
            return response;
        }


        public async Task<ServiceResponse<T>> DeleteRowAsync(int id)
        {
            if (id == 0) throw new ArgumentNullException("id", ErrorCodes.WrongParameter.Text);

            var response = new ServiceResponse<T>();
            try
            {
                int deleted = await _unitOfWork.ExecuteAsync(OrmSqlEnum.DeleteSQL<T>(), new Dictionary<string, object> { { "Id", id } });
                if (deleted != 0)
                {
                    _nlog.WriteLog(NLog.LogLevel.Info, $" Table: {_tableName} tablosunda [{id}] No'lu kayıt silindi.!");
                    response.IsSuccessful = true;
                }
                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{id}] No'lu {ErrorCodes.KayitYok.Text}");

            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Description:{ex.Message}!", ex);

            }
            return response;
        }


        public async Task<ServiceResponse<T>> InsertAsync(T entity)
        {
            var response = new ServiceResponse<T>();
            response.Entity = entity;
            if (entity == null) throw new ArgumentNullException("entity", "Add to DB null entity");
            try
            {
                var insertQuery = OrmHelper.GenerateInsertQuery();
                int inserted = await _unitOfWork.QueryIntegerAsync(insertQuery, entity);

                if (inserted != 0) response.IsSuccessful = true;

                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{entity.Id}] No'lu {ErrorCodes.KayitYok.Text}");
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description:{ex.Message}!", ex);
            }

            return response;

        }


        public async Task<ServiceResponse<T>> InsertRangeAsync(IEnumerable<T> list)
        {
            if (list == null) throw new ArgumentNullException("entity", "Add to DB null entity");

            var response = new ServiceResponse<T>();
            response.List = list;

            try
            {
                var query = OrmHelper.GenerateInsertQuery();
                int inserted = await _unitOfWork.ExecuteAsync(query, (dynamic)list);

                if (inserted == list.Count()) response.IsSuccessful = true;

                else if (inserted < list.Count()) new DataException($"{_tableName} Tablo'suna Kayıt edilmek istenen {list.Count()}, Başarılı olan {inserted}. {ErrorCodes.BilinmeyenHata.Text}");

                else throw new DataException($"{_tableName} Tablo'suna Insert edilirken {ErrorCodes.BilinmeyenHata.Text}");
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description:{ex.Message}!", ex);
            }
            return response;
        }

        public async Task<ServiceResponse<T>> UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity", "Add to DB null entity");

            var response = new ServiceResponse<T>();
            response.Entity = entity;

            try
            {
                var updateQuery = OrmHelper.GenerateUpdateQuery();
                var updated = await _unitOfWork.ExecuteAsync(updateQuery, entity);

                if (updated == 0) response.IsSuccessful = true;

                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{entity.Id}] No'lu kayıt bulunamadı.!");

            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description:{response.ExceptionMessage}!", ex);
            }
            return response;

        }

        //JOIN için yada ortak bir model tanımlayıp istedigimizi cekeiblirz uymayan icin asla modelde ki ad verilir yada aşağıdaki metottaki gibi mapleme yapılır.
        public async Task<IEnumerable<ProdAndCat>> GetForModel()
        {
            string sqlQuery = @"SELECT p.*, c.parentId ,c.name as categoryname
    from [products] p
    inner join categories c on p.CategoryId = c.Id ";


            return await _unitOfWork.QueryAsync<ProdAndCat>(sqlQuery);
        }

        public async Task<List<Products>> GetProductsOneToMany()
        {
            string sqlQuery = @"SELECT * from [products] p
            inner join categories c on p.CategoryId = c.Id ";


            var res = await _unitOfWork.Context.Connection.QueryAsync<Products, Categories, Products>(sqlQuery,
                (product, category) =>
                {
                    Products prod = product;
                    prod.CategoryId = category.Id;

                    return prod;

                }, splitOn: "Id");

            return res.ToList();
        }


    }
}
