--1. PrcResult에서 성공개수와 실패 개수를 다른(가상) 컬럼으로 분리 결과 : 가상 테이블
SELECT p.SchIdx, p.PrcDate, 
	   CASE p.PrcResult WHEN 1 THEN 1 END AS PrcOk,
	   CASE p.PrcResult WHEN 0 THEN 1 END AS PrcFail
  FROM Process AS p

--2. 합계 집계 결과 : 가상테이블(서브쿼리 : 쿼리 안의 쿼리)
SELECT smr.SchIdx, smr.PrcDate,
	   SUM(smr.PrcOk) AS PrcOKAmount, SUM(smr.PrcFail) AS PrcFailAmount
  FROM (
		SELECT p.SchIdx, p.PrcDate, 
			   CASE p.PrcResult WHEN 1 THEN 1 ELSE 0 END AS PrcOk,
	           CASE p.PrcResult WHEN 0 THEN 1 ELSE 0 END AS PrcFail
		  FROM Process AS p
	   ) AS smr
GROUP BY smr.SchIdx, smr.PrcDate

--3-0. JOIN문
SELECT * 
  FROM Schedules AS sch 
 INNER JOIN Process AS prc
	ON sch.SchIdx = prc.SchIdx

--3-1. 2번 결과(가상테이블)와 Schedules 테이블 조인 → 원하는 결과 도출
SELECT sch.SchIdx, SCH.PlantCode, sch.SchAmount, prc.PrcDate,
	   prc.PrcOKAmount, prc.PrcFailAmount
  FROM Schedules AS sch
 INNER JOIN (
	SELECT smr.SchIdx, smr.PrcDate,
		   SUM(smr.PrcOk) AS PrcOKAmount, SUM(smr.PrcFail) AS PrcFailAmount
      FROM (
		    SELECT p.SchIdx, p.PrcDate, 
				   CASE p.PrcResult WHEN 1 THEN 1 ELSE 0 END AS PrcOk,
	               CASE p.PrcResult WHEN 0 THEN 1 ELSE 0 END AS PrcFail
		      FROM Process AS p
		   ) AS smr
	  GROUP BY smr.SchIdx, smr.PrcDate 
 ) AS prc
   ON sch.SchIdx = prc.SchIdx
WHERE sch.PlantCode = 'PC010002'
  AND prc.PrcDate BETWEEN '2021-06-29' AND '2021-07-01'