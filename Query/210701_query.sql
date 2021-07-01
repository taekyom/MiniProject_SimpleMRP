--1. PrcResult���� ���������� ���� ������ �ٸ�(����) �÷����� �и� ��� : ���� ���̺�
SELECT p.SchIdx, p.PrcDate, 
	   CASE p.PrcResult WHEN 1 THEN 1 END AS PrcOk,
	   CASE p.PrcResult WHEN 0 THEN 1 END AS PrcFail
  FROM Process AS p

--2. �հ� ���� ��� : �������̺�(�������� : ���� ���� ����)
SELECT smr.SchIdx, smr.PrcDate,
	   SUM(smr.PrcOk) AS PrcOKAmount, SUM(smr.PrcFail) AS PrcFailAmount
  FROM (
		SELECT p.SchIdx, p.PrcDate, 
			   CASE p.PrcResult WHEN 1 THEN 1 ELSE 0 END AS PrcOk,
	           CASE p.PrcResult WHEN 0 THEN 1 ELSE 0 END AS PrcFail
		  FROM Process AS p
	   ) AS smr
GROUP BY smr.SchIdx, smr.PrcDate

--3-0. JOIN��
SELECT * 
  FROM Schedules AS sch 
 INNER JOIN Process AS prc
	ON sch.SchIdx = prc.SchIdx

--3-1. 2�� ���(�������̺�)�� Schedules ���̺� ���� �� ���ϴ� ��� ����
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